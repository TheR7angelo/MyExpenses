import sqlite3
import argparse
import os
import re
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
        self.relations = []


def get_primary_key_name(cur, table_name):
    try:
        cur.execute(f'PRAGMA table_info("{table_name}")')
        for row in cur.fetchall():
            if row[5] >= 1: return row[1]
    except:
        pass
    return "id"


def load_schema(db):
    if not os.path.exists(db):
        print(f"Error: File not found {db}")
        exit(1)
    conn = sqlite3.connect(db)
    cur = conn.cursor()
    schema = Schema()

    # Tables
    cur.execute("SELECT name FROM sqlite_schema WHERE type = 'table' AND name NOT LIKE 'sqlite_%'")
    tables = [r[0] for r in cur.fetchall()]
    for table in tables:
        cur.execute(f'PRAGMA table_info("{table}")')
        cols = {}
        for cid, name, typ, notnull, dflt, pk in cur.fetchall():
            cols[name] = Column(nullable=not (notnull or pk > 0), reason="table constraint", default=dflt, dtype=typ,
                                pk=(pk > 0))
        schema.tables[table] = cols

    # Relations
    for table in tables:
        cur.execute(f'PRAGMA foreign_key_list("{table}")')
        for row in cur.fetchall():
            local_col, ref_table, ref_col = row[3], row[2], row[4]
            if not ref_col: ref_col = get_primary_key_name(cur, ref_table)
            if local_col in schema.tables[table]:
                schema.tables[table][local_col].fk = f"{ref_table}({ref_col})"
                schema.dependencies[table].add(ref_table)
                schema.usage_map[ref_table].add(table)
                schema.relations.append(f'"{ref_table}" ||--o{{ "{table}" : "fk"')

    # Views
    cur.execute("SELECT name, sql FROM sqlite_schema WHERE type ='view'")
    for name, sql in cur.fetchall():
        schema.views_sql[name] = sql

    # Triggers
    cur.execute("SELECT name, tbl_name, sql FROM sqlite_schema WHERE type = 'trigger'")
    for name, table, sql in cur.fetchall():
        schema.triggers.append({"name": name, "table": table, "sql": sql})
        schema.usage_map[table].add(name)

    return schema


def resolve_views(schema):
    remaining = dict(schema.views_sql)
    for _ in range(10):
        if not remaining: break
        for name, sql in list(remaining.items()):
            try:
                parsed = sqlglot.parse_one(sql)
                select = parsed.find(exp.Select)
                if not select: continue
                sources = {t.alias_or_name: t.name for t in select.find_all(exp.Table)}
                cols = {}
                for proj in select.expressions:
                    alias = proj.alias_or_name
                    cols[alias] = Column(True, "view projection", dtype="VIEW_COL")
                schema.views[name] = cols
                for s_name in sources.values():
                    schema.dependencies[name].add(s_name)
                    schema.usage_map[s_name].add(name)
                    schema.relations.append(f'"{s_name}" ..> "{name}" : "defines"')
                del remaining[name]
            except:
                pass


def generate_mermaid(schema):
    lines = ["erDiagram", "    direction LR"]

    def clean_type(t):
        if not t:
            return "TEXT"
        return t.split("(")[0].upper()

    # Tables
    for table, cols in schema.tables.items():
        lines.append(f'    "{table}" {{')
        for name, col in cols.items():
            dtype = clean_type(col.dtype)
            pk = "PK" if col.pk else ""
            lines.append(f"        {dtype} {name} {pk}")
        lines.append("    }")

    # Views (optionnel mais propre)
    for view_name, cols in schema.views.items():
        lines.append(f'    "{view_name}" {{')
        for name in cols.keys():
            lines.append(f"        TEXT {name}")
        lines.append("    }")

    # Relations FK (ONLY valid ER syntax)
    seen = set()
    for table, cols in schema.tables.items():
        for col in cols.values():
            if col.fk:
                ref_table = col.fk.split("(")[0]
                rel = f'"{ref_table}" ||--o{{ "{table}" : FK'
                if rel not in seen:
                    lines.append(f"    {rel}")
                    seen.add(rel)

    return "\n".join(lines)


def export_html(schema):
    mermaid_content = generate_mermaid(schema)

    html = [f"""<html><head><meta charset="UTF-8">
    <title>Database Schema Audit</title>
    <script type="module">
        import mermaid from 'https://cdn.jsdelivr.net/npm/mermaid@10/dist/mermaid.esm.min.mjs';
        mermaid.initialize({{ 
            startOnLoad: true, 
            theme: 'base',
            themeVariables: {{
                primaryColor: '#ffffff',
                edgeLabelBackground: '#ffffff',
                lineColor: '#3498db'
            }},
            er: {{ useMaxWidth: false }}
        }});
        window.addEventListener('hashchange', () => {{
        const targetId = window.location.hash.substring(1);
        const targetElement = document.getElementById(targetId);
        if (targetElement) {{
            let parent = targetElement.parentElement;
            while (parent) {{
                if (parent.tagName === 'DETAILS') parent.open = true;
                parent = parent.parentElement;
            }}
        }}
    }});
    </script>
    <style>
        :root {{ --bg-color: #f4f7f6; --card-bg: #ffffff; --text-main: #333333; --text-secondary: #7f8c8d; --border-color: #eeeeee; --table-header: #f8f9fa; --code-bg: #f0f0f0; --shadow: rgba(0,0,0,0.1); --type-bg: #f0f0f0; --accent: #3498db; --danger: #d9534f; --success: #5cb85c; }}
        body.dark-mode {{ --bg-color: #1a1a1a; --card-bg: #2d2d2d; --text-main: #e0e0e0; --text-secondary: #b0b0b0; --border-color: #404040; --table-header: #383838; --code-bg: #444444; --shadow: rgba(0,0,0,0.3); --type-bg: #3d3d3d; }}
        body {{ font-family: sans-serif; background: var(--bg-color); color: var(--text-main); padding: 20px; padding-bottom: 120px; transition: 0.3s; scroll-behavior: smooth; }}

        .theme-toggle {{ position: fixed; top: 20px; right: 20px; padding: 10px 15px; border-radius: 20px; background: var(--card-bg); border: 1px solid var(--border-color); color: var(--text-main); cursor: pointer; z-index: 2000; box-shadow: 0 2px 5px var(--shadow); }}

        .bottom-nav {{ position: fixed; bottom: 0; left: 0; right: 0; background: var(--card-bg); padding: 15px 30px; border-top: 1px solid var(--border-color); display: flex; align-items: center; justify-content: center; gap: 40px; z-index: 1001; box-shadow: 0 -2px 10px var(--shadow); }}
        .nav-links {{ display: flex; gap: 20px; }}
        .nav-links a {{ text-decoration: none; color: var(--accent); font-weight: bold; font-size: 0.9em; }}

        .search-wrapper {{ position: relative; width: 350px; }}
        .search-box {{ width: 100%; padding: 10px 40px 10px 20px; border-radius: 20px; border: 1px solid var(--border-color); background: var(--bg-color); color: var(--text-main); outline: none; }}
        .clear-btn {{ position: absolute; right: 12px; top: 50%; transform: translateY(-50%); background: none; border: none; color: var(--text-secondary); cursor: pointer; font-size: 1.2em; display: none; }}

        .counter-badge {{ background: var(--accent); color: white; padding: 2px 10px; border-radius: 12px; font-size: 0.6em; vertical-align: middle; margin-left: 10px; }}
        .card {{ background: var(--card-bg); border-radius: 12px; padding: 25px; margin-bottom: 30px; box-shadow: 0 4px 6px var(--shadow); scroll-margin-top: 20px; }}
        .source-block {{ margin-top: 20px; border: 1px solid var(--border-color); border-radius: 12px; padding: 15px; scroll-margin-top: 20px; }}

        table {{ border-collapse: separate; border-spacing: 0; width: 100%; margin-top: 10px; border: 1px solid var(--border-color); border-radius: 10px; overflow: hidden; }}
        th, td {{ padding: 12px; text-align: left; border-bottom: 1px solid var(--border-color); border-right: 1px solid var(--border-color); }}
        th {{ background: var(--table-header); color: var(--text-secondary); font-size: 0.75em; text-transform: uppercase; }}

        .nullable {{ color: var(--danger); font-weight: bold; font-size: 0.8em; }}
        .notnull {{ color: var(--success); font-weight: bold; font-size: 0.8em; }}
        .badge {{ padding: 4px 8px; border-radius: 5px; font-size: 0.7em; font-weight: bold; margin-left: 5px; display: inline-block; text-decoration: none; }}
        .badge-pk {{ background: #f1c40f; color: #000; }}
        .badge-fk {{ background: #3498db; color: #fff; }}
        .dep-tag {{ display: inline-block; padding: 3px 12px; background: var(--type-bg); border-radius: 15px; font-size: 0.75em; margin: 2px; color: var(--text-secondary); text-decoration: none; }}
        .dep-tag:hover {{ background: var(--accent); color: white; }}

        pre {{ background: var(--code-bg); padding: 15px; border-radius: 8px; overflow-x: auto; font-size: 0.9em; border: 1px solid var(--border-color); color: var(--text-main); }}
        summary {{ list-style: none; display: flex; align-items: center; cursor: pointer; outline: none; font-weight: bold; font-size: 1.2em; }}
        summary::before {{ content: "▶"; display: inline-block; width: 1.5em; color: var(--text-secondary); font-size: 0.8em; }}
        details[open] > summary::before {{ content: "▼"; }}
        .type-label {{ font-family: monospace; background: var(--type-bg); padding: 2px 6px; border-radius: 4px; color: var(--text-secondary); font-size: 0.85em; }}

.graph-container {{
    background: white; 
    border-radius: 12px; 
    padding: 0; 
    overflow: auto; /* Permet le scroll dans les deux sens */
    height: 700px; 
    border: 1px solid var(--border-color); 
    cursor: grab;
    position: relative;
    width: 100%;
}}

.mermaid {{ 
    margin: 0;
    display: inline-block; /* Très important pour que le conteneur s'adapte à la largeur du SVG */
    padding: 20px;
}}

.graph-container:active {{
    cursor: grabbing;
}}
    </style>
    <script>
        function toggleTheme() {{
            document.body.classList.toggle('dark-mode');
            document.getElementById('tb').innerText = document.body.classList.contains('dark-mode') ? '☀️' : '🌙';
        }}
        function filterResults() {{
            const val = document.getElementById('search').value.toLowerCase();
            document.getElementById('clear-btn').style.display = val ? "block" : "none";
            document.querySelectorAll('.source-block').forEach(b => {{
                b.style.display = b.getAttribute('id').toLowerCase().includes(val) ? "" : "none";
            }});
        }}
        function clearFilter() {{
            document.getElementById('search').value = '';
            filterResults();
        }}
        window.onload = () => {{
            if (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches) document.body.classList.add('dark-mode');
            document.getElementById('tb').innerText = document.body.classList.contains('dark-mode') ? '☀️' : '🌙';

            const vp = document.getElementById('vp');
            let d = false, x, y, sl, st;
            vp.addEventListener('mousedown', e => {{ d = true; x = e.pageX - vp.offsetLeft; y = e.pageY - vp.offsetTop; sl = vp.scrollLeft; st = vp.scrollTop; }});
            vp.addEventListener('mousemove', e => {{ if(!d) return; e.preventDefault(); vp.scrollLeft = sl - (e.pageX - vp.offsetLeft - x); vp.scrollTop = st - (e.pageY - vp.offsetTop - y); }});
            ['mouseup', 'mouseleave'].forEach(ev => vp.addEventListener(ev, () => d = false));
        }};
    </script></head><body>
        <button id="tb" class="theme-toggle" onclick="toggleTheme()">🌙</button>
        <h1 style="text-align:center; margin-top: 40px; margin-bottom: 40px;">Database Schema Audit</h1>

        <div class="bottom-nav">
            <div class="nav-links">
                <a href="#sec-tables">📂 Tables</a>
                <a href="#sec-views">📊 Views</a>
                <a href="#sec-triggers">⚡ Triggers</a>
                <a href="#sec-graph">🗺️ Diagram</a>
            </div>
            <div class="search-wrapper">
                <input type="text" id="search" class="search-box" onkeyup="filterResults()" placeholder="Quick filter...">
                <button id="clear-btn" class="clear-btn" onclick="clearFilter()">&times;</button>
            </div>
        </div>"""]

    def create_block(title, collection, section_id, is_table=False):
        html.append(
            f"<details open class='card' id='{section_id}'><summary>{title} <span class='counter-badge'>{len(collection)}</span></summary>")
        for name in sorted(collection.keys()):
            cols = collection[name]
            html.append(f"<div class='source-block' id='{name}'><h3>{name}</h3>")

            # Dependency Tags
            if name in schema.dependencies or name in schema.usage_map:
                html.append("<div style='margin-bottom:12px;'>")
                if schema.dependencies[name]:
                    html.append("<small style='color:var(--text-secondary); font-weight:bold;'>Uses: </small>")
                    for d in sorted(schema.dependencies[name]):
                        html.append(f"<a href='#{d}' class='dep-tag'>{d}</a>")
                if schema.usage_map[name]:
                    html.append("<br><small style='color:var(--text-secondary); font-weight:bold;'>Used by: </small>")
                    for u in sorted(schema.usage_map[name]):
                        html.append(f"<a href='#{u}' class='dep-tag'>{u}</a>")
                html.append("</div>")

            # Table Column Details
            html.append("<table><thead><tr><th>Column</th><th>Type</th><th>Status</th>")
            if is_table: html.append("<th>Relationship</th><th>Default Value</th>")
            html.append("</tr></thead><tbody>")
            for c, i in cols.items():
                st_class = "nullable" if i.nullable else "notnull"
                st_label = "NULLABLE" if i.nullable else "NOT NULL"
                row = f"<tr><td><strong>{c}</strong></td><td><span class='type-label'>{i.dtype}</span></td>"
                row += f"<td><span class='{st_class}'>{st_label}</span></td>"
                if is_table:
                    pk_b = "<span class='badge badge-pk'>PK</span>" if i.pk else ""
                    fk_b = f"<a href='#{i.fk.split('(')[0]}' class='badge badge-fk'>FK → {i.fk}</a>" if i.fk else ""
                    row += f"<td>{pk_b}{fk_b}</td><td><span style='font-family:monospace; font-size:0.85em;'>{i.default or ''}</span></td>"
                html.append(row + "</tr>")
            html.append("</tbody></table></div>")
        html.append("</details>")

    create_block("Source Tables", schema.tables, "sec-tables", is_table=True)
    create_block("Analyzed Views", schema.views, "sec-views", is_table=False)

    if schema.triggers:
        html.append(
            f"<details open class='card' id='sec-triggers'><summary>Triggers <span class='counter-badge'>{len(schema.triggers)}</span></summary>")
        for t in sorted(schema.triggers, key=lambda x: x['name']):
            html.append(
                f"<div class='source-block' id='{t['name']}'><h3>⚡ {t['name']} <small style='color:var(--text-secondary);'>(on {t['table']})</small></h3><pre><code>{t['sql']}</code></pre></div>")
        html.append("</details>")

    # Graph Section
    html.append(f"""<div class='card' id='sec-graph'>
        <summary>🗺️ Interactive ER Diagram</summary>
        <div class="graph-container" id="vp">
            <pre class="mermaid">{mermaid_content}</pre>
        </div>
    </div></body></html>""")

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