import sqlite3
import json
import argparse
from collections import defaultdict
import sqlglot
from sqlglot import exp


class Column:
    def __init__(self, nullable=True, reason="unknown", sources=None, default=None):
        self.nullable = nullable
        self.reason = reason
        self.sources = sources or []
        self.default = default


class Schema:
    def __init__(self):
        self.tables = {}
        self.views_sql = {}
        self.views = {}
        self.dependencies = defaultdict(set)


def load_schema(db):
    conn = sqlite3.connect(db)
    cur = conn.cursor()

    schema = Schema()

    cur.execute("""
                SELECT name
                FROM sqlite_schema
                WHERE type = 'table'
                  AND name NOT LIKE 'sqlite_%'
                """)

    for (table,) in cur.fetchall():
        cur.execute(f"PRAGMA table_info({table})")
        cols = {}

        for cid, name, typ, notnull, dflt, pk in cur.fetchall():
            nullable = not (notnull or pk)
            cols[name] = Column(nullable, "table constraint", default=dflt)

        schema.tables[table] = cols

    cur.execute("""
                SELECT name, sql
                FROM sqlite_schema
                WHERE type ='view'
                """)

    for name, sql in cur.fetchall():
        schema.views_sql[name] = sql

    return schema


def extract_sources(select):
    sources = {}

    for table in select.find_all(exp.Table):
        sources[table.alias_or_name] = table.name

    return sources


def detect_left_join(select):
    nullable = set()

    for join in select.find_all(exp.Join):

        if join.args.get("kind") == "left":
            nullable.add(join.this.alias_or_name)

    return nullable


def resolve_star(schema, sources):
    cols = {}

    for alias, table in sources.items():

        if table in schema.tables:

            for c, info in schema.tables[table].items():
                cols[c] = info

        if table in schema.views:

            for c, info in schema.views[table].items():
                cols[c] = info

    return cols


def infer_expr(expr, schema, sources, nullable_tables):
    if isinstance(expr, exp.Column):
        table_alias = expr.table
        col = expr.name
        if table_alias in sources:
            table = sources[table_alias]
            if table in schema.tables:
                base = schema.tables[table].get(col)
                if base:
                    is_nullable = True if table_alias in nullable_tables else base.nullable
                    return Column(is_nullable, "source column", [f"{table}.{col}"])
            if table in schema.views:
                base = schema.views[table].get(col)
                if base:
                    return base
        return Column(True, "unknown column")

    if isinstance(expr, (exp.Sum, exp.Avg, exp.Min, exp.Max)):
        return Column(True, "aggregate (nullable if empty)")

    if isinstance(expr, exp.Coalesce):
        for arg in expr.expressions:
            info = infer_expr(arg, schema, sources, nullable_tables)
            if not info.nullable:
                return Column(False, "COALESCE (guaranteed by argument)")
        if isinstance(expr.expressions[-1], exp.Literal):
            return Column(False, "COALESCE (default value)")
        return Column(True, "COALESCE (all inputs nullable)")

    if isinstance(expr, exp.Case):
        is_nullable = False
        branches = [expr.args.get("default")] + [e.args.get("value") for e in expr.args.get("ifs", [])]
        for branch in branches:
            if branch:
                info = infer_expr(branch, schema, sources, nullable_tables)
                if info.nullable:
                    is_nullable = True
                    break
        return Column(is_nullable, "CASE expression")

    if isinstance(expr, (exp.Cast, exp.Round, exp.Paren)):
        return infer_expr(expr.this, schema, sources, nullable_tables)

    if isinstance(expr, exp.Binary):
        left = infer_expr(expr.left, schema, sources, nullable_tables)
        right = infer_expr(expr.right, schema, sources, nullable_tables)
        return Column(left.nullable or right.nullable, "binary op", left.sources + right.sources)

    if isinstance(expr, exp.Literal):
        return Column(False, "literal constant")

    return Column(True, "complex expression")


def analyze_select(select, schema):
    sources = extract_sources(select)
    nullable_tables = detect_left_join(select)

    cols = {}

    for proj in select.expressions:

        if isinstance(proj, exp.Star):
            cols.update(resolve_star(schema, sources))
            continue

        alias = proj.alias_or_name
        expr = proj.this if isinstance(proj, exp.Alias) else proj

        info = infer_expr(expr, schema, sources, nullable_tables)

        cols[alias] = info

    return cols


def analyze_query(node, schema):
    if isinstance(node, exp.Select):
        return analyze_select(node, schema)

    if isinstance(node, exp.Union):

        left = analyze_query(node.left, schema)
        right = analyze_query(node.right, schema)

        merged = {}

        for c in left:

            if c in right:
                nullable = left[c].nullable or right[c].nullable

                merged[c] = Column(nullable, "UNION")

        return merged

    select = node.find(exp.Select)

    if select:
        return analyze_select(select, schema)

    return {}


def analyze_view(name, sql, schema):
    parsed = sqlglot.parse_one(sql)

    if isinstance(parsed, exp.With):
        parsed = parsed.this

    cols = analyze_query(parsed, schema)

    schema.views[name] = cols

    for t in parsed.find_all(exp.Table):
        schema.dependencies[name].add(t.name)

    return cols


def resolve_views(schema):
    remaining = dict(schema.views_sql)

    while remaining:

        progress = False

        for name, sql in list(remaining.items()):

            try:

                analyze_view(name, sql, schema)

                del remaining[name]

                progress = True

            except Exception:
                pass

        if not progress:
            break


def report(schema):
    problems = []

    for view, cols in schema.views.items():

        print(f"\nVIEW {view}")

        for c, info in cols.items():

            status = "NULLABLE" if info.nullable else "NOT_NULL"

            print(f"{view}.{c:<25} {status:<10} {info.reason}")

            if info.nullable:
                problems.append((view, c))

    return problems


def export_json(schema):
    data = {"tables": {}, "views": {}}

    for table in sorted(schema.tables.keys()):
        data["tables"][table] = {
            c: {
                "nullable": info.nullable,
                "reason": info.reason,
                "default": info.default  # Ajout ici
            } for c, info in schema.tables[table].items()
        }

    # On trie juste les noms de vues
    for view in sorted(schema.views.keys()):
        data["views"][view] = {c: {"nullable": info.nullable, "reason": info.reason}
                               for c, info in schema.views[view].items()}

    with open("audit_report.json", "w") as f:
        json.dump(data, f, indent=2)


def export_graph(schema):
    with open("audit_report.dot", "w") as f:
        f.write("digraph views {\n")
        f.write('  rankdir=TB;\n')
        f.write('  node [shape=box, style=filled];\n')

        # 1. Tables triées
        for t in sorted(schema.tables.keys()):
            f.write(f'  "{t}" [fillcolor=lightgrey];\n')

        # 2. Vues triées
        for v in sorted(schema.views.keys()):
            f.write(f'  "{v}" [fillcolor=lightblue];\n')

        # 3. Liens triés (source -> cible)
        for v in sorted(schema.dependencies.keys()):
            for d in sorted(schema.dependencies[v]):
                if d in schema.views_sql or d in schema.tables:
                    f.write(f'  "{d}" -> "{v}";\n')

        f.write("}\n")


def export_html(schema):
    html = ["""<html><head>
        <meta charset="UTF-8">
        <style>
            :root {
                --bg-color: #f4f7f6;
                --card-bg: #ffffff;
                --text-main: #333333;
                --text-secondary: #7f8c8d;
                --text-title: #2c3e50;
                --border-color: #eeeeee;
                --table-header: #f8f9fa;
                --code-bg: #f0f0f0;
                --shadow: rgba(0,0,0,0.1);
            }

            /* On définit les couleurs sombres dans une classe à part */
            body.dark-mode {
                --bg-color: #1a1a1a;
                --card-bg: #2d2d2d;
                --text-main: #e0e0e0;
                --text-secondary: #b0b0b0;
                --text-title: #ffffff;
                --border-color: #404040;
                --table-header: #383838;
                --code-bg: #444444;
                --shadow: rgba(0,0,0,0.3);
            }

            body { font-family: sans-serif; background: var(--bg-color); color: var(--text-main); padding: 20px; transition: background 0.3s, color 0.3s; }
            .card { background: var(--card-bg); border-radius: 12px; padding: 20px; margin-bottom: 20px; box-shadow: 0 2px 4px var(--shadow); border: none; }
            h1 { color: var(--text-title); text-align: center; margin-bottom: 30px; } 

            .theme-toggle {
                position: fixed; top: 20px; right: 20px;
                padding: 10px 15px; border-radius: 20px;
                border: 1px solid var(--border-color);
                background: var(--card-bg); color: var(--text-main);
                cursor: pointer; font-weight: bold; z-index: 1000; box-shadow: 0 2px 5px var(--shadow);
            }

            .group-header { cursor: pointer; list-style: none; outline: none; padding: 5px 0; }
            .group-header::-webkit-details-marker { display: none; }
            .title-wrapper { display: flex; justify-content: space-between; align-items: center; width: 100%; }
            .group-header h2 { margin: 0; color: var(--text-title); }

            .group-header h2::before { content: "▶"; display: inline-block; width: 1.5em; font-size: 0.7em; transition: transform 0.2s; }
            details[open] > .group-header h2::before { content: "▼"; }

            details.source-block { margin-top: 15px; border: 1px solid var(--border-color); border-radius: 12px; padding: 12px; }
            summary.source-title { font-weight: bold; cursor: pointer; list-style: none; outline: none; font-size: 1.1em; color: var(--text-title); }
            summary.source-title::before { content: "▶"; display: inline-block; width: 1.5em; font-size: 0.8em; }
            details[open] > summary.source-title::before { content: "▼"; }

            table { 
                border-collapse: separate; border-spacing: 0; width: 100%; margin-top: 15px; 
                background: var(--card-bg); border: 1px solid var(--border-color); border-radius: 10px; overflow: hidden; 
            }
            th, td { border-bottom: 1px solid var(--border-color); border-right: 1px solid var(--border-color); padding: 12px; text-align: left; }
            th { background-color: var(--table-header); color: var(--text-secondary); font-size: 0.9em; text-transform: uppercase; border-bottom: 2px solid var(--border-color); }

            th:last-child, td:last-child { border-right: none; }
            tr:last-child td { border-bottom: none; }

            .nullable { color: #d9534f; font-weight: bold; }
            .notnull { color: #5cb85c; font-weight: bold; }
            code { background: var(--code-bg); padding: 2px 6px; border-radius: 4px; font-family: monospace; }
            .count-badge { background: #34495e; color: white; padding: 4px 12px; border-radius: 20px; font-size: 0.8em; }
        </style>
        <script>
            // Fonction pour mettre à jour le texte du bouton
            function updateBtnText() {
                const isDark = document.body.classList.contains('dark-mode');
                document.getElementById('theme-btn').innerText = isDark ? '☀️ Light Mode' : '🌙 Dark Mode';
            }

            // Bascule manuelle
            function toggleTheme() {
                document.body.classList.toggle('dark-mode');
                updateBtnText();
            }

            // Détection automatique au chargement
            window.onload = function() {
                if (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches) {
                    document.body.classList.add('dark-mode');
                }
                updateBtnText();
            };
        </script>
    </head><body>
        <button id="theme-btn" class="theme-toggle" onclick="toggleTheme()">🌙 Dark Mode</button>
        <h1>Global Schema Audit</h1>"""]

    def create_block(title, collection, is_table=False):
        html.append(f"""
            <details open class='card'>
                <summary class='group-header'>
                    <div class='title-wrapper'>
                        <h2>{title}</h2>
                        <span class='count-badge'>{len(collection)}</span>
                    </div>
                </summary>""")

        if not collection:
            html.append("<p style='color: var(--text-secondary); padding-left: 20px;'>No data found.</p></details>")
            return

        for source in sorted(collection.keys()):
            cols = collection[source]
            html.append(f"<details open class='source-block'><summary class='source-title'>{source}</summary>")

            headers = ["Column", "Status", "Reason"]
            if is_table:
                headers.append("Default")

            header_html = "".join([f"<th>{h}</th>" for h in headers])
            html.append(f"<table><thead><tr>{header_html}</tr></thead><tbody>")

            for c, info in cols.items():
                status_class = "nullable" if info.nullable else "notnull"
                status_text = "NULLABLE" if info.nullable else "NOT_NULL"

                row = f"<tr><td><strong>{c}</strong></td><td class='{status_class}'>{status_text}</td><td>{info.reason}</td>"

                if is_table:
                    if info.default is not None:
                        row += f"<td><code>{info.default}</code></td>"
                    else:
                        row += "<td></td>"

                row += "</tr>"
                html.append(row)

            html.append("</tbody></table></details>")
        html.append("</details>")

    create_block("Source Tables", schema.tables, is_table=True)
    create_block("Analyzed Views", schema.views, is_table=False)

    html.append("</body></html>")

    with open("audit_report.html", "w", encoding="utf-8") as f:
        f.write("\n".join(html))


def main():
    parser = argparse.ArgumentParser(
        description="Analyseur de schéma SQLite : audit de nullabilité et génération de rapports.",
        formatter_class=argparse.ArgumentDefaultsHelpFormatter
    )

    parser.add_argument("db", help="Chemin vers le fichier database.db/.sqlite")
    parser.add_argument("--json", action="store_true", help="Génère audit_report.json")
    parser.add_argument("--html", action="store_true", help="Génère audit_report.html")
    parser.add_argument("--graph", action="store_true", help="Génère audit_report.dot (Graphviz)")
    parser.add_argument("--fail-on-nullable", action="store_true", help="Retourne code 1 si une colonne est NULLABLE")

    args = parser.parse_args()

    schema = load_schema(args.db)

    resolve_views(schema)

    problems = report(schema)

    if args.json:
        export_json(schema)

    if args.html:
        export_html(schema)

    if args.graph:
        export_graph(schema)

    if args.fail_on_nullable and problems:
        exit(1)


if __name__ == "__main__":
    main()
