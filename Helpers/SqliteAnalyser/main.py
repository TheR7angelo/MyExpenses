import sqlite3
import json
import argparse
from collections import defaultdict
import sqlglot
from sqlglot import exp


class Column:
    def __init__(self, nullable=True, reason="unknown", sources=None, default=None, dtype="UNKNOWN"):
        self.nullable = nullable
        self.reason = reason
        self.sources = sources or []
        self.default = default
        self.dtype = dtype  # Nouveau champ pour le type


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
            # On récupère le type (typ) ici
            cols[name] = Column(nullable, "table constraint", default=dflt, dtype=typ)

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
                    return Column(is_nullable, "source column", [f"{table}.{col}"], dtype=base.dtype)
            if table in schema.views:
                base = schema.views[table].get(col)
                if base:
                    return base
        return Column(True, "unknown column", dtype="UNKNOWN")

    if isinstance(expr, (exp.Sum, exp.Avg, exp.Min, exp.Max)):
        return Column(True, "aggregate", dtype="NUMERIC")

    if isinstance(expr, exp.Coalesce):
        dtype = "UNKNOWN"
        for arg in expr.expressions:
            info = infer_expr(arg, schema, sources, nullable_tables)
            dtype = info.dtype  # On prend le type du premier argument connu
            if not info.nullable:
                return Column(False, "COALESCE (guaranteed)", dtype=dtype)
        return Column(True, "COALESCE", dtype=dtype)

    if isinstance(expr, exp.Case):
        # On simplifie : on prend le type de la première branche
        first_branch = expr.args.get("default") or expr.args.get("ifs")[0].args.get("value")
        info_branch = infer_expr(first_branch, schema, sources, nullable_tables)
        return Column(True, "CASE expression", dtype=info_branch.dtype)

    if isinstance(expr, (exp.Cast, exp.Round, exp.Paren)):
        info = infer_expr(expr.this, schema, sources, nullable_tables)
        if isinstance(expr, exp.Cast):
            info.dtype = str(expr.args.get("to")).upper()
        return info

    if isinstance(expr, exp.Literal):
        dtype = "TEXT" if expr.is_string else "NUMBER"
        return Column(False, "literal constant", dtype=dtype)

    return Column(True, "complex expression", dtype="UNKNOWN")


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
                merged[c] = Column(left[c].nullable or right[c].nullable, "UNION", dtype=left[c].dtype)
        return merged
    select = node.find(exp.Select)
    return analyze_select(select, schema) if select else {}


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
            except:
                pass
        if not progress: break


def report(schema):
    problems = []
    for view, cols in schema.views.items():
        print(f"\nVIEW {view}")
        for c, info in cols.items():
            status = "NULLABLE" if info.nullable else "NOT_NULL"
            print(f"{view}.{c:<25} {status:<10} [{info.dtype}] {info.reason}")
            if info.nullable: problems.append((view, c))
    return problems


def export_json(schema):
    data = {"tables": {}, "views": {}}
    for table in sorted(schema.tables.keys()):
        data["tables"][table] = {
            c: {"nullable": i.nullable, "type": i.dtype, "reason": i.reason, "default": i.default}
            for c, i in schema.tables[table].items()
        }
    for view in sorted(schema.views.keys()):
        data["views"][view] = {
            c: {"nullable": i.nullable, "type": i.dtype, "reason": i.reason}
            for c, i in schema.views[view].items()
        }
    with open("audit_report.json", "w") as f:
        json.dump(data, f, indent=2)


def export_html(schema):
    html = ["""<html><head>
        <meta charset="UTF-8">
        <style>
            :root {
                --bg-color: #f4f7f6; --card-bg: #ffffff; --text-main: #333333;
                --text-secondary: #7f8c8d; --text-title: #2c3e50; --border-color: #eeeeee;
                --table-header: #f8f9fa; --code-bg: #f0f0f0; --shadow: rgba(0,0,0,0.1);
            }
            body.dark-mode {
                --bg-color: #1a1a1a; --card-bg: #2d2d2d; --text-main: #e0e0e0;
                --text-secondary: #b0b0b0; --text-title: #ffffff; --border-color: #404040;
                --table-header: #383838; --code-bg: #444444; --shadow: rgba(0,0,0,0.3);
            }
            body { font-family: sans-serif; background: var(--bg-color); color: var(--text-main); padding: 20px; transition: 0.3s; }
            .card { background: var(--card-bg); border-radius: 12px; padding: 20px; margin-bottom: 20px; box-shadow: 0 2px 4px var(--shadow); }
            h1 { text-align: center; color: var(--text-title); }

            .search-container { display: flex; justify-content: center; align-items: center; gap: 10px; margin-bottom: 30px; }
            .search-box { width: 350px; padding: 12px 20px; border-radius: 25px; border: 1px solid var(--border-color); background: var(--card-bg); color: var(--text-main); outline: none; }
            .clear-btn { padding: 10px 15px; border-radius: 20px; border: 1px solid var(--border-color); background: var(--card-bg); color: var(--text-secondary); cursor: pointer; font-size: 0.8em; font-weight: bold; }
            .clear-btn:hover { background: #d9534f; color: white; }

            summary { list-style: none; display: flex; align-items: center; cursor: pointer; outline: none; }
            summary::-webkit-details-marker { display: none; }
            summary * { display: inline; }
            summary::before { content: "▶"; display: inline-block; width: 1.5em; font-size: 0.8em; color: var(--text-secondary); transition: 0.2s; }
            details[open] > summary::before { content: "▼"; }

            .source-block { margin-top: 15px; border: 1px solid var(--border-color); border-radius: 12px; padding: 12px; }
            .source-title { font-weight: bold; font-size: 1.1em; color: var(--text-title); }

            table { border-collapse: separate; border-spacing: 0; width: 100%; margin-top: 10px; border: 1px solid var(--border-color); border-radius: 10px; overflow: hidden; }
            th, td { padding: 12px; text-align: left; border-bottom: 1px solid var(--border-color); border-right: 1px solid var(--border-color); }
            th { background: var(--table-header); color: var(--text-secondary); font-size: 0.8em; text-transform: uppercase; }
            th:last-child, td:last-child { border-right: none; }

            .nullable { color: #d9534f; font-weight: bold; }
            .notnull { color: #5cb85c; font-weight: bold; }
            .type-label { font-family: monospace; background: var(--code-bg); padding: 2px 6px; border-radius: 4px; font-size: 0.9em; color: var(--text-secondary); }
            .count-badge { background: #34495e; color: white; padding: 4px 12px; border-radius: 20px; font-size: 0.6em; margin-left: 10px; }

            .theme-toggle { position: fixed; top: 20px; right: 20px; padding: 10px 15px; border-radius: 20px; background: var(--card-bg); color: var(--text-main); cursor: pointer; border: 1px solid var(--border-color); font-weight: bold; z-index: 1000; }
        </style>
        <script>
            function updateBtnText() {
                const isDark = document.body.classList.contains('dark-mode');
                document.getElementById('theme-btn').innerText = isDark ? '☀️ Light' : '🌙 Dark';
            }
            function toggleTheme() {
                document.body.classList.toggle('dark-mode');
                updateBtnText();
            }
            function filterResults() {
                let input = document.getElementById('search').value.toLowerCase();
                document.querySelectorAll('.source-block').forEach(block => {
                    let name = block.querySelector('.source-title').innerText.toLowerCase();
                    block.style.display = name.includes(input) ? "" : "none";
                });
            }
            function clearSearch() { document.getElementById('search').value = ''; filterResults(); }

            // Détection auto et init
            window.onload = function() {
                if (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches) {
                    document.body.classList.add('dark-mode');
                }
                updateBtnText();
            };
        </script>
    </head><body>
        <button id="theme-btn" class="theme-toggle" onclick="toggleTheme()">🌙 Dark</button>
        <h1>Global Schema Audit</h1>
        <div class="search-container">
            <input type="text" id="search" class="search-box" onkeyup="filterResults()" placeholder="Search table or view...">
            <button class="clear-btn" onclick="clearSearch()">Clear Filter</button>
        </div>"""]

    def create_block(title, collection, is_table=False):
        # "open" ajouté ici pour que les sections Tables/Vues soient dépliées
        html.append(
            f"<details open class='card'><summary><h2>{title} <span class='count-badge'>{len(collection)}</span></h2></summary>")

        if not collection:
            html.append("<p style='color: var(--text-secondary); padding-left: 20px;'>No data found.</p></details>")
            return

        for source in sorted(collection.keys()):
            cols = collection[source]
            # "open" ajouté ici aussi pour chaque table/vue individuelle
            html.append(f"<details open class='source-block'><summary class='source-title'>{source}</summary><table>")
            html.append("<thead><tr><th>Column</th><th>Type</th><th>Status</th><th>Reason</th>")
            if is_table: html.append("<th>Default</th>")
            html.append("</tr></thead><tbody>")

            for c, i in cols.items():
                status_class = "nullable" if i.nullable else "notnull"
                status_text = "NULLABLE" if i.nullable else "NOT_NULL"
                row = f"<tr><td><strong>{c}</strong></td><td><span class='type-label'>{i.dtype}</span></td>"
                row += f"<td class='{status_class}'>{status_text}</td><td>{i.reason}</td>"
                if is_table:
                    row += f"<td><code>{i.default if i.default is not None else ''}</code></td>"
                html.append(row + "</tr>")
            html.append("</tbody></table></details>")
        html.append("</details>")

    create_block("Source Tables", schema.tables, is_table=True)
    create_block("Analyzed Views", schema.views, is_table=False)
    html.append("</body></html>")

    with open("audit_report.html", "w", encoding="utf-8") as f:
        f.write("\n".join(html))


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("db")
    parser.add_argument("--json", action="store_true")
    parser.add_argument("--html", action="store_true")
    args = parser.parse_args()

    schema = load_schema(args.db)
    resolve_views(schema)
    report(schema)

    if args.json: export_json(schema)
    if args.html: export_html(schema)


if __name__ == "__main__":
    main()