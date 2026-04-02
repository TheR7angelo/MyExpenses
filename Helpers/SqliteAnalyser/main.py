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
        self.triggers = []
        self.dependencies = defaultdict(set)
        self.usage_map = defaultdict(set)


def get_primary_key_name(cur, table_name):
    cur.execute(f"PRAGMA table_info({table_name})")
    for row in cur.fetchall():
        if row[5] >= 1: return row[1]
    return "id"


def load_schema(db):
    conn = sqlite3.connect(db)
    cur = conn.cursor()
    schema = Schema()

    cur.execute("SELECT name FROM sqlite_schema WHERE type = 'table' AND name NOT LIKE 'sqlite_%'")
    tables = [r[0] for r in cur.fetchall()]
    for table in tables:
        cur.execute(f"PRAGMA table_info({table})")
        cols = {}
        for cid, name, typ, notnull, dflt, pk in cur.fetchall():
            cols[name] = Column(nullable=not (notnull or pk > 0), reason="table constraint", default=dflt, dtype=typ,
                                pk=(pk > 0))
        schema.tables[table] = cols

    for table in tables:
        cur.execute(f"PRAGMA foreign_key_list({table})")
        for row in cur.fetchall():
            local_col, ref_table, ref_col = row[3], row[2], row[4]
            if not ref_col: ref_col = get_primary_key_name(cur, ref_table)
            if local_col in schema.tables[table]:
                schema.tables[table][local_col].fk = f"{ref_table}({ref_col})"
                schema.dependencies[table].add(ref_table)
                schema.usage_map[ref_table].add(table)

    cur.execute("SELECT name, sql FROM sqlite_schema WHERE type ='view'")
    for name, sql in cur.fetchall():
        schema.views_sql[name] = sql

    cur.execute("SELECT name, tbl_name, sql FROM sqlite_schema WHERE type = 'trigger'")
    for name, table, sql in cur.fetchall():
        schema.triggers.append({"name": name, "table": table, "sql": sql})
        schema.usage_map[table].add(name)

    return schema


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
    return cols, sources


def analyze_query(node, schema):
    if isinstance(node, exp.Select): return analyze_select(node, schema)
    if isinstance(node, exp.Union):
        l_cols, l_src = analyze_query(node.left, schema)
        r_cols, r_src = analyze_query(node.right, schema)
        combined_src = {**l_src, **r_src}
        res_cols = {c: Column(l_cols[c].nullable or r_cols[c].nullable, "UNION", dtype=l_cols[c].dtype)
                    for c in l_cols if c in r_cols}
        return res_cols, combined_src
    select = node.find(exp.Select)
    return analyze_select(select, schema) if select else ({}, {})


def resolve_views(schema):
    remaining = dict(schema.views_sql)
    for _ in range(10):
        if not remaining: break
        for name, sql in list(remaining.items()):
            try:
                parsed = sqlglot.parse_one(sql)
                cols, sources = analyze_query(parsed, schema)
                schema.views[name] = cols
                for s_name in sources.values():
                    schema.dependencies[name].add(s_name)
                    schema.usage_map[s_name].add(name)
                del remaining[name]
            except:
                pass


def export_html(schema):
    html = ["""<html><head><meta charset="UTF-8"><style>
        :root { --bg-color: #f4f7f6; --card-bg: #ffffff; --text-main: #333333; --text-secondary: #7f8c8d; --border-color: #eeeeee; --table-header: #f8f9fa; --code-bg: #f0f0f0; --shadow: rgba(0,0,0,0.1); --type-bg: #f0f0f0; --accent: #3498db; }
        body.dark-mode { --bg-color: #1a1a1a; --card-bg: #2d2d2d; --text-main: #e0e0e0; --text-secondary: #b0b0b0; --border-color: #404040; --table-header: #383838; --code-bg: #444444; --shadow: rgba(0,0,0,0.3); --type-bg: #3d3d3d; }
        body { font-family: sans-serif; background: var(--bg-color); color: var(--text-main); padding: 20px; transition: 0.3s; scroll-behavior: smooth; }
        .card { background: var(--card-bg); border-radius: 12px; padding: 20px; margin-bottom: 20px; box-shadow: 0 2px 4px var(--shadow); }
        .search-wrapper { position: relative; width: 400px; margin: 0 auto 30px auto; }
        .search-box { width: 100%; padding: 12px 45px 12px 20px; border-radius: 25px; border: 1px solid var(--border-color); background: var(--card-bg); color: var(--text-main); outline: none; box-sizing: border-box; }
        .clear-btn { position: absolute; right: 15px; top: 50%; transform: translateY(-50%); background: none; border: none; color: var(--text-secondary); cursor: pointer; font-size: 1.2em; display: none; }
        summary { list-style: none; display: flex; align-items: center; cursor: pointer; outline: none; font-weight: bold; }
        summary::before { content: "▶"; display: inline-block; width: 1.5em; color: var(--text-secondary); transition: 0.2s; }
        details[open] > summary::before { content: "▼"; }
        .source-block { margin-top: 15px; border: 1px solid var(--border-color); border-radius: 12px; padding: 12px; scroll-margin-top: 80px; transition: border-color 0.5s; }

        /* Highlight animation when clicking an anchor */
        .source-block:target { border-color: var(--accent); border-width: 2px; animation: flash 1.5s ease-out; }
        @keyframes flash { 0% { background-color: rgba(52, 152, 219, 0.2); } 100% { background-color: transparent; } }

        table { border-collapse: separate; border-spacing: 0; width: 100%; margin-top: 10px; border: 1px solid var(--border-color); border-radius: 10px; overflow: hidden; }
        th, td { padding: 10px; text-align: left; border-bottom: 1px solid var(--border-color); border-right: 1px solid var(--border-color); }
        th { background: var(--table-header); color: var(--text-secondary); font-size: 0.8em; text-transform: uppercase; }
        .nullable { color: #d9534f; font-weight: bold; }
        .notnull { color: #5cb85c; font-weight: bold; }
        .badge { padding: 3px 8px; border-radius: 4px; font-size: 0.7em; font-weight: bold; margin-left: 5px; display: inline-block; text-decoration: none; }
        .badge-pk { background: #f1c40f; color: #000; }
        .badge-fk { background: #3498db; color: #fff; }
        .dep-tag { display: inline-block; padding: 2px 10px; background: var(--type-bg); border-radius: 15px; font-size: 0.75em; margin: 2px; color: var(--text-secondary); text-decoration: none; border: 1px solid transparent; transition: 0.2s; }
        .dep-tag:hover { background: var(--accent); color: white; }
        .default-val { background: #e8f4fd; color: #2980b9; padding: 2px 6px; border-radius: 4px; font-family: monospace; font-size: 0.85em; border: 1px solid #d1e9f9; }
        body.dark-mode .default-val { background: #1b3a4b; color: #a5d8ff; border-color: #244b5f; }
        .type-label { font-family: monospace; background: var(--type-bg); padding: 2px 5px; border-radius: 4px; color: var(--text-secondary); }
        .theme-toggle { position: fixed; top: 20px; right: 20px; padding: 10px 15px; border-radius: 20px; background: var(--card-bg); border: 1px solid var(--border-color); color: var(--text-main); cursor: pointer; z-index: 1000; }
        pre { background: var(--code-bg); padding: 15px; border-radius: 8px; overflow-x: auto; font-size: 0.9em; border: 1px solid var(--border-color); color: var(--text-main); }
    </style><script>
        function updateBtn() {
            const isDark = document.body.classList.contains('dark-mode');
            document.getElementById('tb').innerText = isDark ? '☀️' : '🌙';
        }
        function toggleTheme() {
            document.body.classList.toggle('dark-mode');
            updateBtn();
        }
        function filterResults() {
            const val = document.getElementById('search').value.toLowerCase();
            document.getElementById('clear-btn').style.display = val ? "block" : "none";
            document.querySelectorAll('.source-block').forEach(b => {
                const name = b.getAttribute('id').toLowerCase();
                b.style.display = name.includes(val) ? "" : "none";
            });
        }

        window.addEventListener('hashchange', function() {
            const id = location.hash.substring(1);
            const target = document.getElementById(id);
            if (target) {
                let parent = target.parentElement;
                while (parent) {
                    if (parent.tagName === 'DETAILS') parent.open = true;
                    parent = parent.parentElement;
                }
            }
        });

        window.onload = () => {
            if (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches) {
                document.body.classList.add('dark-mode');
            }
            updateBtn();
        };
    </script></head><body>
        <button id="tb" class="theme-toggle" onclick="toggleTheme()">🌙</button>
        <h1 style="text-align:center">Global Schema Audit</h1>
        <div class="search-wrapper">
            <input type="text" id="search" class="search-box" onkeyup="filterResults()" placeholder="Search tables, views or triggers...">
            <button id="clear-btn" class="clear-btn" onclick="document.getElementById('search').value='';filterResults()">&times;</button>
        </div>"""]

    def create_block(title, collection, is_table=False):
        html.append(f"<details open class='card'><summary><h2>{title} ({len(collection)})</h2></summary>")
        for name in sorted(collection.keys()):
            cols = collection[name]
            html.append(f"<div class='source-block' id='{name}'><h3>{name}</h3>")

            if name in schema.dependencies or name in schema.usage_map:
                html.append("<div style='margin-bottom:10px;'>")
                if schema.dependencies[name]:
                    html.append("<small style='color:var(--text-secondary)'>Uses: </small>")
                    for d in sorted(schema.dependencies[name]):
                        html.append(f"<a href='#{d}' class='dep-tag'>{d}</a>")
                if schema.usage_map[name]:
                    html.append("<br><small style='color:var(--text-secondary)'>Used by: </small>")
                    for u in sorted(schema.usage_map[name]):
                        html.append(f"<a href='#{u}' class='dep-tag'>{u}</a>")
                html.append("</div>")

            html.append("<table><thead><tr><th>Column</th><th>Type</th><th>Status</th>")
            if is_table: html.append("<th>Relationship</th><th>Default Value</th>")
            html.append("</tr></thead><tbody>")
            for c, i in cols.items():
                st = "nullable" if i.nullable else "notnull"
                row = f"<tr><td><strong>{c}</strong></td><td><span class='type-label'>{i.dtype}</span></td>"
                row += f"<td class='{st}'>{'NULLABLE' if i.nullable else 'NOT_NULL'}</td>"
                if is_table:
                    pk_b = "<span class='badge badge-pk'>PRIMARY KEY</span>" if i.pk else ""
                    fk_b = f"<a href='#{i.fk.split('(')[0]}' class='badge badge-fk'>FOREIGN KEY → {i.fk}</a>" if i.fk else ""
                    d_val = f"<span class='default-val'>{i.default or ''}</span>" if i.default is not None else ""
                    row += f"<td>{pk_b}{fk_b}</td><td>{d_val}</td>"
                html.append(row + "</tr>")
            html.append("</tbody></table></div>")
        html.append("</details>")

    create_block("Source Tables", schema.tables, is_table=True)
    create_block("Analyzed Views", schema.views, is_table=False)

    if schema.triggers:
        html.append(f"<details open class='card'><summary><h2>Triggers ({len(schema.triggers)})</h2></summary>")
        for t in sorted(schema.triggers, key=lambda x: x['name']):
            html.append(
                f"<div class='source-block' id='{t['name']}'><h3>Trigger: {t['name']} <small>(on {t['table']})</small></h3>")
            html.append(f"<pre><code>{t['sql']}</code></pre></div>")
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
        print("Audit report successfully generated: audit_report.html")