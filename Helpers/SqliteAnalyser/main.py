import sqlite3
import json
import argparse
from collections import defaultdict
import sqlglot
from sqlglot import exp


class Column:
    def __init__(self, nullable=True, reason="unknown", sources=None):
        self.nullable = nullable
        self.reason = reason
        self.sources = sources or []


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
    WHERE type='table'
    AND name NOT LIKE 'sqlite_%'
    """)

    for (table,) in cur.fetchall():

        cur.execute(f"PRAGMA table_info({table})")

        cols = {}

        for cid, name, typ, notnull, dflt, pk in cur.fetchall():

            nullable = not (notnull or pk)

            cols[name] = Column(nullable, "table constraint")

        schema.tables[table] = cols

    cur.execute("""
    SELECT name, sql
    FROM sqlite_schema
    WHERE type='view'
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

                    if table_alias in nullable_tables:
                        return Column(True, "LEFT JOIN", [f"{table}.{col}"])

                    return Column(base.nullable, base.reason, [f"{table}.{col}"])

            if table in schema.views:

                base = schema.views[table].get(col)

                if base:

                    if table_alias in nullable_tables:
                        return Column(True, "LEFT JOIN view", [f"{table}.{col}"])

                    return base

        return Column(True, "unknown column")

    if isinstance(expr, exp.Coalesce):
        return Column(False, "COALESCE")

    if isinstance(expr, exp.Count):
        return Column(False, "COUNT")

    if isinstance(expr, (exp.Sum, exp.Avg, exp.Min, exp.Max)):
        return Column(True, "aggregate")

    if isinstance(expr, exp.Literal):
        return Column(False, "literal")

    if isinstance(expr, exp.Binary):

        left = infer_expr(expr.left, schema, sources, nullable_tables)
        right = infer_expr(expr.right, schema, sources, nullable_tables)

        nullable = left.nullable or right.nullable

        return Column(nullable, "binary", left.sources + right.sources)

    if isinstance(expr, exp.Case):
        return Column(True, "CASE")

    return Column(True, "complex")


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

    # On trie juste les noms de tables
    for table in sorted(schema.tables.keys()):
        # On itère directement sur les colonnes, l'ordre d'origine est préservé
        data["tables"][table] = {c: {"nullable": info.nullable, "reason": info.reason}
                                 for c, info in schema.tables[table].items()}

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
    # La liste est créée ICI, à l'intérieur de la fonction
    html = ["""<html><head><style>
        body { font-family: sans-serif; background: #f4f7f6; padding: 20px; }
        .card { background: white; border-radius: 8px; padding: 20px; margin-bottom: 20px; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }
        h1 { color: #333; } h2 { border-bottom: 2px solid #eee; padding-bottom: 10px; }
        table { border-collapse: collapse; width: 100%; margin-top: 10px; }
        th, td { border: 1px solid #eee; padding: 10px; text-align: left; }
        th { background-color: #f8f9fa; }
        .nullable { color: #d9534f; font-weight: bold; }
        .notnull { color: #5cb85c; font-weight: bold; }
    </style></head><body><h1>Audit Schéma Global</h1>"""]

    # Générateur de bloc pour une collection (Tables ou Vues)
    def create_block(title, collection):
        html.append(f"<div class='card'><h2>{title} ({len(collection)})</h2>")
        if not collection:
            html.append("<p>Aucune donnée trouvée.</p></div>")
            return

        # On trie uniquement les noms des tables/vues
        for source in sorted(collection.keys()):
            cols = collection[source]
            html.append(f"<h3>{source}</h3><table><tr><th>Colonne</th><th>Status</th><th>Raison</th></tr>")

            # ICI : Pas de sorted(), on garde l'ordre original
            for c, info in cols.items():
                status_class = "nullable" if info.nullable else "notnull"
                status_text = "NULLABLE" if info.nullable else "NOT_NULL"
                html.append(
                    f"<tr><td>{c}</td><td class='{status_class}'>{status_text}</td><td>{info.reason}</td></tr>"
                )
            html.append("</table>")
        html.append("</div>")

    # Appel des blocs
    create_block("Tables Sources", schema.tables)
    create_block("Vues Analysées", schema.views)

    html.append("</body></html>")

    # Écriture finale
    with open("audit_report.html", "w") as f:
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