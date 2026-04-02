import sqlite3
import json
import argparse
from collections import defaultdict
import sqlglot
from sqlglot import exp


class Column:
    def __init__(self, nullable=True, reason="unknown", sources=None, default=None, dtype="UNKNOWN", pk=False, fk=None):
        self.nullable = nullable
        self.reason = reason
        self.sources = sources or []
        self.default = default
        self.dtype = dtype
        self.pk = pk
        self.fk = fk


class Schema:
    def __init__(self):
        self.tables = {}
        self.views_sql = {}
        self.views = {}
        self.triggers = []  # Nouvelle liste pour stocker les triggers
        self.dependencies = defaultdict(set)


def get_primary_key_name(cur, table_name):
    cur.execute(f"PRAGMA table_info({table_name})")
    for row in cur.fetchall():
        if row[5] >= 1: return row[1]
    return "id"


def load_schema(db):
    conn = sqlite3.connect(db)
    cur = conn.cursor()
    schema = Schema()

    # Tables
    cur.execute("SELECT name FROM sqlite_schema WHERE type = 'table' AND name NOT LIKE 'sqlite_%'")
    tables = [r[0] for r in cur.fetchall()]
    for table in tables:
        cur.execute(f"PRAGMA table_info({table})")
        cols = {}
        for cid, name, typ, notnull, dflt, pk in cur.fetchall():
            cols[name] = Column(nullable=not (notnull or pk > 0), reason="table constraint", default=dflt, dtype=typ,
                                pk=(pk > 0))
        schema.tables[table] = cols

    # Foreign Keys
    for table in tables:
        cur.execute(f"PRAGMA foreign_key_list({table})")
        for row in cur.fetchall():
            local_col, ref_table, ref_col = row[3], row[2], row[4]
            if not ref_col: ref_col = get_primary_key_name(cur, ref_table)
            if local_col in schema.tables[table]:
                schema.tables[table][local_col].fk = f"{ref_table}({ref_col})"

    # Views
    cur.execute("SELECT name, sql FROM sqlite_schema WHERE type ='view'")
    for name, sql in cur.fetchall():
        schema.views_sql[name] = sql

    # Triggers (Nouvelle section)
    cur.execute("SELECT name, tbl_name, sql FROM sqlite_schema WHERE type = 'trigger'")
    for name, table, sql in cur.fetchall():
        schema.triggers.append({
            "name": name,
            "table": table,
            "sql": sql
        })

    return schema


# --- [Fonctions d'analyse SQL identiques au précédent] ---

def extract_sources(select):
    sources = {}
    for table in select.find_all(exp.Table): sources[table.alias_or_name] = table.name
    return sources


def detect_left_join(select):
    nullable = set()
    for join in select.find_all(exp.Join):
        if join.args.get("kind") == "left": nullable.add(join.this.alias_or_name)
    return nullable


def resolve_star(schema, sources):
    cols = {}
    for alias, table in sources.items():
        data = schema.tables.get(table) or schema.views.get(table)
        if data: cols.update(data)
    return cols


def infer_expr(expr, schema, sources, nullable_tables):
    if isinstance(expr, exp.Column):
        table_alias, col = expr.table, expr.name
        if table_alias in sources:
            table = sources[table_alias]
            base_cols = schema.tables.get(table) or schema.views.get(table)
            if base_cols and col in base_cols:
                base = base_cols[col]
                is_nullable = True if table_alias in nullable_tables else base.nullable
                return Column(is_nullable, "source column", dtype=base.dtype)
        return Column(True, "unknown column", dtype="UNKNOWN")
    if isinstance(expr, (exp.Sum, exp.Avg, exp.Min, exp.Max)): return Column(True, "aggregate", dtype="NUMERIC")
    if isinstance(expr, exp.Literal): return Column(False, "literal", dtype="TEXT" if expr.is_string else "NUMBER")
    return Column(True, "expression", dtype="UNKNOWN")


def analyze_select(select, schema):
    sources = extract_sources(select)
    nullable_tables = detect_left_join(select)
    cols = {}
    for proj in select.expressions:
        if isinstance(proj, exp.Star): cols.update(resolve_star(schema, sources)); continue
        alias = proj.alias_or_name
        expr = proj.this if isinstance(proj, exp.Alias) else proj
        cols[alias] = infer_expr(expr, schema, sources, nullable_tables)
    return cols


def analyze_query(node, schema):
    if isinstance(node, exp.Select): return analyze_select(node, schema)
    if isinstance(node, exp.Union):
        left, right = analyze_query(node.left, schema), analyze_query(node.right, schema)
        return {c: Column(left[c].nullable or right[c].nullable, "UNION", dtype=left[c].dtype) for c in left if
                c in right}
    select = node.find(exp.Select)
    return analyze_select(select, schema) if select else {}


def analyze_view(name, sql, schema):
    parsed = sqlglot.parse_one(sql)
    schema.views[name] = analyze_query(parsed, schema)
    return schema.views[name]


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


def export_html(schema):
    html = ["""<html><head><meta charset="UTF-8"><style>
        :root { --bg-color: #f4f7f6; --card-bg: #ffffff; --text-main: #333333; --text-secondary: #7f8c8d; --border-color: #eeeeee; --table-header: #f8f9fa; --code-bg: #f0f0f0; --shadow: rgba(0,0,0,0.1); }
        body.dark-mode { --bg-color: #1a1a1a; --card-bg: #2d2d2d; --text-main: #e0e0e0; --text-secondary: #b0b0b0; --border-color: #404040; --table-header: #383838; --code-bg: #444444; --shadow: rgba(0,0,0,0.3); }
        body { font-family: sans-serif; background: var(--bg-color); color: var(--text-main); padding: 20px; transition: 0.3s; }
        .card { background: var(--card-bg); border-radius: 12px; padding: 20px; margin-bottom: 20px; box-shadow: 0 2px 4px var(--shadow); }

        .search-wrapper { position: relative; width: 400px; margin: 0 auto 30px auto; }
        .search-box { width: 100%; padding: 12px 45px 12px 20px; border-radius: 25px; border: 1px solid var(--border-color); background: var(--card-bg); color: var(--text-main); outline: none; box-sizing: border-box; }
        .clear-btn { position: absolute; right: 15px; top: 50%; transform: translateY(-50%); background: none; border: none; color: var(--text-secondary); cursor: pointer; font-size: 1.2em; display: none; padding: 0; line-height: 1; }
        .clear-btn:hover { color: #d9534f; }

        summary { list-style: none; display: flex; align-items: center; cursor: pointer; outline: none; }
        summary::-webkit-details-marker { display: none; }
        summary::before { content: "▶"; display: inline-block; width: 1.5em; color: var(--text-secondary); }
        details[open] > summary::before { content: "▼"; }
        .source-block { margin-top: 15px; border: 1px solid var(--border-color); border-radius: 12px; padding: 12px; }
        table { border-collapse: separate; border-spacing: 0; width: 100%; margin-top: 10px; border: 1px solid var(--border-color); border-radius: 10px; overflow: hidden; }
        th, td { padding: 10px; text-align: left; border-bottom: 1px solid var(--border-color); border-right: 1px solid var(--border-color); }
        th { background: var(--table-header); color: var(--text-secondary); font-size: 0.8em; text-transform: uppercase; }
        .nullable { color: #d9534f; font-weight: bold; }
        .notnull { color: #5cb85c; font-weight: bold; }
        .badge { padding: 3px 8px; border-radius: 4px; font-size: 0.7em; font-weight: bold; margin-left: 5px; display: inline-block; }
        .badge-pk { background: #f1c40f; color: #000; }
        .badge-fk { background: #3498db; color: #fff; }
        .default-val { background: #e8f4fd; color: #2980b9; padding: 2px 6px; border-radius: 4px; font-family: monospace; font-size: 0.85em; border: 1px solid #d1e9f9; }
        body.dark-mode .default-val { background: #1b3a4b; color: #a5d8ff; border-color: #244b5f; }
        .type-label { font-family: monospace; background: var(--code-bg); padding: 2px 5px; border-radius: 4px; color: var(--text-secondary); }
        .theme-toggle { position: fixed; top: 20px; right: 20px; padding: 10px 15px; border-radius: 20px; background: var(--card-bg); color: var(--text-main); cursor: pointer; border: 1px solid var(--border-color); z-index: 1000; }
        pre { background: var(--code-bg); padding: 15px; border-radius: 8px; overflow-x: auto; font-size: 0.9em; border: 1px solid var(--border-color); color: var(--text-main); }
    </style><script>
        function toggleTheme() { document.body.classList.toggle('dark-mode'); updateBtn(); }
        function updateBtn() { document.getElementById('tb').innerText = document.body.classList.contains('dark-mode') ? '☀️' : '🌙'; }

        function filterResults() {
            const input = document.getElementById('search');
            const clearBtn = document.getElementById('clear-btn');
            const filter = input.value.toLowerCase();
            clearBtn.style.display = input.value.length > 0 ? "block" : "none";

            document.querySelectorAll('.source-block').forEach(b => {
                const title = b.querySelector('summary').innerText.toLowerCase();
                b.style.display = title.includes(filter) ? "" : "none";
            });
        }

        function clearSearch() { 
            const input = document.getElementById('search');
            input.value = ''; 
            filterResults();
            input.focus();
        }

        window.onload = () => { if (window.matchMedia('(prefers-color-scheme: dark)').matches) document.body.classList.add('dark-mode'); updateBtn(); };
    </script></head><body>
        <button id="tb" class="theme-toggle" onclick="toggleTheme()">🌙</button>
        <h1 style="text-align:center">Global Schema Audit</h1>
        <div class="search-wrapper">
            <input type="text" id="search" class="search-box" onkeyup="filterResults()" placeholder="Search table, view or trigger...">
            <button id="clear-btn" class="clear-btn" onclick="clearSearch()">&times;</button>
        </div>"""]

    def create_block(title, collection, is_table=False):
        html.append(f"<details open class='card'><summary><h2>{title} ({len(collection)})</h2></summary>")
        for source in sorted(collection.keys()):
            cols = collection[source]
            html.append(f"<details open class='source-block'><summary><strong>{source}</strong></summary><table>")
            headers = ["Column", "Type", "Status"]
            if is_table: headers += ["Relationship", "Default Value"]
            html.append(f"<thead><tr>{''.join(f'<th>{h}</th>' for h in headers)}</tr></thead><tbody>")

            for c, i in cols.items():
                st = "nullable" if i.nullable else "notnull"
                row = f"<tr><td><strong>{c}</strong></td><td><span class='type-label'>{i.dtype}</span></td>"
                row += f"<td class='{st}'>{'NULLABLE' if i.nullable else 'NOT_NULL'}</td>"
                if is_table:
                    pk_b = "<span class='badge badge-pk'>PRIMARY KEY</span>" if i.pk else ""
                    fk_b = f"<span class='badge badge-fk'>FOREIGN KEY → {i.fk}</span>" if i.fk else ""
                    d_val = f"<span class='default-val'>{i.default}</span>" if i.default is not None else ""
                    row += f"<td>{pk_b}{fk_b}</td><td>{d_val}</td>"
                html.append(row + "</tr>")
            html.append("</tbody></table></details>")
        html.append("</details>")

    # Section Tables & Views
    create_block("Source Tables", schema.tables, is_table=True)
    create_block("Analyzed Views", schema.views, is_table=False)

    # Section Triggers (Nouvelle méthode d'affichage)
    if schema.triggers:
        html.append(
            f"<details open class='card'><summary><h2>Database Triggers ({len(schema.triggers)})</h2></summary>")
        for trig in sorted(schema.triggers, key=lambda x: x['name']):
            html.append(
                f"<details open class='source-block'><summary><strong>{trig['name']}</strong> (on {trig['table']})</summary>")
            html.append(f"<pre><code>{trig['sql']}</code></pre>")
            html.append("</details>")
        html.append("</details>")

    html.append("</body></html>")
    with open("audit_report.html", "w", encoding="utf-8") as f:
        f.write("\n".join(html))


if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument("db")
    parser.add_argument("--html", action="store_true")
    args = parser.parse_args()

    schema_data = load_schema(args.db)
    resolve_views(schema_data)

    if args.html:
        export_html(schema_data)
        print("Audit report generated: audit_report.html")