import sqlite3
import argparse
import os
import re
import json
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

    def to_dict(self):
        return {"nullable": self.nullable, "dtype": self.dtype, "default": self.default, "pk": self.pk, "fk": self.fk}


class Schema:
    def __init__(self):
        self.tables = {}
        self.table_indexes = defaultdict(list)
        self.views_sql = {}
        self.views = {}
        self.triggers = []
        self.dependencies = defaultdict(set)
        self.usage_map = defaultdict(set)
        self.relations = []
        self.warnings = []


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
        print(f"Error: File not found {db}");
        exit(1)
    conn = sqlite3.connect(db)
    cur = conn.cursor()
    schema = Schema()

    cur.execute("SELECT name FROM sqlite_schema WHERE type = 'table' AND name NOT LIKE 'sqlite_%'")
    tables = [r[0] for r in cur.fetchall()]
    for table in tables:
        cur.execute(f'PRAGMA table_info("{table}")')
        cols = {}
        has_pk = False
        for cid, name, typ, notnull, dflt, pk in cur.fetchall():
            is_pk = (pk > 0)
            if is_pk: has_pk = True
            cols[name] = Column(nullable=not (notnull or is_pk), default=dflt, dtype=typ, pk=is_pk)
        schema.tables[table] = cols
        if not has_pk: schema.warnings.append(f"Table <strong>{table}</strong> : pas de PK.")

        cur.execute(f'PRAGMA index_list("{table}")')
        for idx in cur.fetchall():
            idx_name, is_unique = idx[1], idx[2]
            cur.execute(f'PRAGMA index_info("{idx_name}")')
            idx_cols = [r[2] for r in cur.fetchall()]
            schema.table_indexes[table].append({"name": idx_name, "unique": bool(is_unique), "cols": idx_cols})
            for cn in idx_cols:
                if cn in schema.tables[table]: schema.tables[table][cn].reason = "INDEXED"

    for table in tables:
        cur.execute(f'PRAGMA foreign_key_list("{table}")')
        for row in cur.fetchall():
            l_col, r_tab, r_col = row[3], row[2], row[4]
            if not r_col: r_col = get_primary_key_name(cur, r_tab)
            if l_col in schema.tables[table]:
                schema.tables[table][l_col].fk = f"{r_tab}({r_col})"
                schema.dependencies[table].add(r_tab)
                schema.usage_map[r_tab].add(table)
                if schema.tables[table][l_col].reason != "INDEXED":
                    schema.warnings.append(f"Performance : FK <strong>{table}.{l_col}</strong> not indexed.")

    cur.execute("SELECT name, sql FROM sqlite_schema WHERE type ='view'")
    for name, sql in cur.fetchall(): schema.views_sql[name] = sql

    cur.execute("SELECT name, tbl_name, sql FROM sqlite_schema WHERE type = 'trigger'")
    for name, table, sql in cur.fetchall():
        schema.triggers.append({"name": name, "table": table, "sql": sql})
        schema.usage_map[table].add(name)

    conn.close()
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
                    final_type = "UNKNOWN"
                    final_nullable = True

                    cast_node = proj.find(exp.Cast)
                    if cast_node: final_type = cast_node.args['to'].this

                    col_node = proj.find(exp.Column)
                    if col_node:
                        src_tab = sources.get(
                            col_node.table or (list(sources.keys())[0] if len(sources) == 1 else None))
                        if src_tab in schema.tables and col_node.name in schema.tables[src_tab]:
                            src_col = schema.tables[src_tab][col_node.name]
                            final_nullable = src_col.nullable
                            if final_type == "UNKNOWN": final_type = src_col.dtype

                    cols[alias] = Column(nullable=final_nullable, dtype=final_type)

                schema.views[name] = cols
                for s_name in sources.values():
                    schema.dependencies[name].add(s_name)
                    schema.usage_map[s_name].add(name)
                del remaining[name]
            except:
                pass


def generate_mermaid(schema):
    lines = ["erDiagram"]
    clean = lambda t: re.sub(r'[^a-zA-Z0-9]', '_', str(t).split('(')[0]).upper() if t else "TEXT"
    for t, cs in schema.tables.items():
        lines.append(f'    "{t}" {{')
        for n, c in cs.items():
            b = []
            if c.pk: b.append("PK")
            if c.fk: b.append("FK")
            lines.append(f"        {clean(c.dtype)} {n} {' '.join(b)}")
        lines.append("    }")
    for v, cs in schema.views.items():
        lines.append(f'    "{v}" {{')
        for n, c in cs.items(): lines.append(f"        {clean(c.dtype)} {n}")
        lines.append("    }")
    seen = set()
    for t, cs in schema.tables.items():
        for cn, ci in cs.items():
            if ci.fk:
                rt = ci.fk.split("(")[0]
                rel = f'"{rt}" ||--o{{ "{t}" : "{cn}"'
                if rel not in seen: lines.append(f"    {rel}"); seen.add(rel)
    return "\n".join(lines)


def export_html(schema):
    mmd = generate_mermaid(schema)
    html = [f"""<html><head><meta charset="UTF-8"><title>Database Schema Audit</title>
    <link rel="icon" href="data:image/svg+xml,<svg xmlns=%22http://www.w3.org/2000/svg%22 viewBox=%220 0 100 100%22><text y=%22.9em%22 font-size=%2290%22>🗄️</text></svg>">
    <script type="module">
        import mermaid from 'https://cdn.jsdelivr.net/npm/mermaid@10/dist/mermaid.esm.min.mjs';
        mermaid.initialize({{ startOnLoad: true, theme: 'base', themeVariables: {{ primaryColor: '#ffffff', edgeLabelBackground: '#ffffff', lineColor: '#3498db' }}, er: {{ useMaxWidth: false }} }});
        window.addEventListener('hashchange', () => {{
            const tid = window.location.hash.substring(1);
            const el = document.getElementById(tid);
            if (el) {{ let p = el.parentElement; while (p) {{ if (p.tagName === 'DETAILS') p.open = true; p = p.parentElement; }} }}
        }});
    </script>
    <style>
        :root {{ --bg-color: #f4f7f6; --card-bg: #ffffff; --text-main: #333333; --text-secondary: #7f8c8d; --border-color: #eeeeee; --table-header: #f8f9fa; --code-bg: #f0f0f0; --shadow: rgba(0,0,0,0.1); --type-bg: #f0f0f0; --accent: #3498db; --danger: #d9534f; --success: #5cb85c; --warn-bg: #fff5f5; }}
        body.dark-mode {{ --bg-color: #1a1a1a; --card-bg: #2d2d2d; --text-main: #e0e0e0; --text-secondary: #b0b0b0; --border-color: #404040; --table-header: #383838; --code-bg: #444444; --shadow: rgba(0,0,0,0.3); --type-bg: #3d3d3d; --warn-bg: #3d2a2a; }}
        body {{ font-family: sans-serif; background: var(--bg-color); color: var(--text-main); padding: 20px; padding-bottom: 120px; transition: 0.3s; scroll-behavior: smooth; }}

        /* Animation de clignotement lors du clic sur un lien */
        .source-block:target {{ animation: highlight 2s ease-out; border: 2px solid var(--accent) !important; }}
        @keyframes highlight {{ 0% {{ background-color: rgba(52, 152, 219, 0.3); }} 100% {{ background-color: transparent; }} }}

        .theme-toggle {{ position: fixed; top: 20px; right: 20px; padding: 10px 15px; border-radius: 20px; background: var(--card-bg); border: 1px solid var(--border-color); color: var(--text-main); cursor: pointer; z-index: 2000; box-shadow: 0 2px 5px var(--shadow); }}
        .bottom-nav {{ position: fixed; bottom: 0; left: 0; right: 0; background: var(--card-bg); padding: 15px 30px; border-top: 1px solid var(--border-color); display: flex; align-items: center; justify-content: center; gap: 40px; z-index: 1001; box-shadow: 0 -2px 10px var(--shadow); }}
        .nav-links {{ display: flex; gap: 20px; }}
        .nav-links a {{ text-decoration: none; color: var(--accent); font-weight: bold; font-size: 0.9em; }}
        .search-wrapper {{ position: relative; width: 350px; }}
        .search-box {{ width: 100%; padding: 10px 40px 10px 20px; border-radius: 20px; border: 1px solid var(--border-color); background: var(--bg-color); color: var(--text-main); outline: none; }}
        .clear-btn {{ position: absolute; right: 12px; top: 50%; transform: translateY(-50%); background: none; border: none; color: var(--text-secondary); cursor: pointer; font-size: 1.2em; display: none; }}
        .counter-badge {{ background: var(--accent); color: white; padding: 2px 10px; border-radius: 12px; font-size: 0.6em; vertical-align: middle; margin-left: 10px; }}
        .card {{ background: var(--card-bg); border-radius: 12px; padding: 25px; margin-bottom: 30px; box-shadow: 0 4px 6px var(--shadow); scroll-margin-top: 20px; }}
        .warning-card {{ border-left: 5px solid var(--danger); background: var(--warn-bg); }}
        .source-block {{ margin-top: 20px; border: 1px solid var(--border-color); border-radius: 12px; padding: 15px; scroll-margin-top: 20px; transition: 0.3s; }}
        table {{ border-collapse: separate; border-spacing: 0; width: 100%; margin-top: 10px; border: 1px solid var(--border-color); border-radius: 10px; overflow: hidden; }}
        th, td {{ padding: 12px; text-align: left; border-bottom: 1px solid var(--border-color); border-right: 1px solid var(--border-color); }}
        th {{ background: var(--table-header); color: var(--text-secondary); font-size: 0.75em; text-transform: uppercase; }}
        .nullable {{ color: var(--danger); font-weight: bold; font-size: 0.8em; }}
        .notnull {{ color: var(--success); font-weight: bold; font-size: 0.8em; }}
        .badge {{ padding: 4px 8px; border-radius: 5px; font-size: 0.7em; font-weight: bold; margin-left: 5px; display: inline-block; text-decoration: none; color: white; transition: 0.2s; }}
        .badge:hover {{ transform: scale(1.1); filter: brightness(1.1); }}
        .badge-pk {{ background: #f1c40f; color: #000; }} .badge-fk {{ background: #3498db; }} .badge-idx {{ background: #9b59b6; }}

        /* STYLE DES DÉPENDANCES AVEC SURVOL */
        .dep-tag {{ display: inline-block; padding: 3px 12px; background: var(--type-bg); border-radius: 15px; font-size: 0.75em; margin: 2px; color: var(--text-secondary); text-decoration: none; transition: all 0.2s ease-in-out; border: 1px solid transparent; }}
        .dep-tag:hover {{ background: var(--accent); color: white; transform: scale(1.05); box-shadow: 0 2px 5px var(--shadow); border-color: rgba(255,255,255,0.2); }}

        pre {{ background: var(--code-bg); padding: 15px; border-radius: 8px; overflow-x: auto; font-size: 0.9em; border: 1px solid var(--border-color); color: var(--text-main); }}
        summary {{ list-style: none; display: flex; align-items: center; cursor: pointer; outline: none; font-weight: bold; font-size: 1.2em; }}
        summary::before {{ content: "▶"; display: inline-block; width: 1.5em; color: var(--text-secondary); font-size: 0.8em; }}
        details[open] > summary::before {{ content: "▼"; }}
        .type-label {{ font-family: monospace; background: var(--type-bg); padding: 2px 6px; border-radius: 4px; color: var(--text-secondary); font-size: 0.85em; }}
        .graph-container {{ background: white; border-radius: 12px; padding: 0; overflow: auto; height: 700px; border: 1px solid var(--border-color); cursor: grab; position: relative; width: 100%; }}
        .mermaid {{ margin: 0; display: inline-block; padding: 20px; }}
    </style>
    <script>
        function toggleTheme() {{ document.body.classList.toggle('dark-mode'); document.getElementById('tb').innerText = document.body.classList.contains('dark-mode') ? '☀️' : '🌙'; }}
        function filterResults() {{
            const val = document.getElementById('search').value.toLowerCase();
            document.getElementById('clear-btn').style.display = val ? "block" : "none";
            document.querySelectorAll('.source-block').forEach(b => {{ b.style.display = b.getAttribute('id').toLowerCase().includes(val) ? "" : "none"; }});
        }}
        function clearFilter() {{ document.getElementById('search').value = ''; filterResults(); }}
        window.onload = () => {{
            if (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches) document.body.classList.add('dark-mode');
            document.getElementById('tb').innerText = document.body.classList.contains('dark-mode') ? '☀️' : '🌙';
            const vp = document.getElementById('vp'); let d = false, x, y, sl, st;
            vp.addEventListener('mousedown', e => {{ d = true; x = e.pageX - vp.offsetLeft; y = e.pageY - vp.offsetTop; sl = vp.scrollLeft; st = vp.scrollTop; }});
            vp.addEventListener('mousemove', e => {{ if(!d) return; e.preventDefault(); vp.scrollLeft = sl - (e.pageX - vp.offsetLeft - x); vp.scrollTop = st - (e.pageY - vp.offsetTop - y); }});
            ['mouseup', 'mouseleave'].forEach(ev => vp.addEventListener(ev, () => d = false));
        }};
    </script></head><body>
        <button id="tb" class="theme-toggle" onclick="toggleTheme()">🌙</button>
        <h1 style="text-align:center; margin-top: 40px; margin-bottom: 40px;">Database Schema Audit</h1>
        <div class="bottom-nav">
            <div class="nav-links"><a href="#sec-warnings">⚠️ Warnings</a><a href="#sec-tables">📂 Tables</a><a href="#sec-views">📊 Views</a><a href="#sec-triggers">⚡ Triggers</a><a href="#sec-graph">🗺️ Diagram</a></div>
            <div class="search-wrapper"><input type="text" id="search" class="search-box" onkeyup="filterResults()" placeholder="Filter..."><button id="clear-btn" class="clear-btn" onclick="clearFilter()">&times;</button></div>
        </div>"""]

    # Warnings
    html.append(
        f"<details open class='card warning-card' id='sec-warnings'><summary>Warnings <span class='counter-badge'>{len(schema.warnings)}</span></summary><ul>")
    for w in schema.warnings: html.append(f"<li>{w}</li>")
    html.append("</ul></details>")

    def create_block(title, col_dict, sid, is_tab=False):
        html.append(
            f"<details open class='card' id='{sid}'><summary>{title} <span class='counter-badge'>{len(col_dict)}</span></summary>")
        for name in sorted(col_dict.keys()):
            cols = col_dict[name]
            html.append(f"<div class='source-block' id='{name}'><h3>{name}</h3>")
            if name in schema.dependencies or name in schema.usage_map:
                html.append("<div style='margin-bottom:12px;'>")
                for d in sorted(schema.dependencies[name]): html.append(f"<a href='#{d}' class='dep-tag'>Uses: {d}</a>")
                for u in sorted(schema.usage_map[name]): html.append(f"<a href='#{u}' class='dep-tag'>Used by: {u}</a>")
                html.append("</div>")
            html.append("<table><thead><tr><th>Column</th><th>Type</th><th>Status</th>")
            if is_tab: html.append("<th>Meta</th><th>Default</th>")
            html.append("</tr></thead><tbody>")
            for c, i in cols.items():
                st_cl, st_lb = ("nullable", "NULLABLE") if i.nullable else ("notnull", "NOT NULL")
                row = f"<tr><td><strong>{c}</strong></td><td><span class='type-label'>{i.dtype}</span></td><td><span class='{st_cl}'>{st_lb}</span></td>"
                if is_tab:
                    meta = ("<span class='badge badge-pk'>PK</span>" if i.pk else "") + (
                        f"<a href='#{i.fk.split('(')[0]}' class='badge badge-fk'>FK</a>" if i.fk else "") + (
                               "<span class='badge badge-idx'>IDX</span>" if i.reason == "INDEXED" else "")
                    row += f"<td>{meta}</td><td><small>{i.default or ''}</small></td>"
                html.append(row + "</tr>")
            html.append("</tbody></table>")
            if is_tab and name in schema.table_indexes:
                idx_s = " | ".join(
                    [f"<code>{x['name']}</code>({','.join(x['cols'])})" for x in schema.table_indexes[name]])
                html.append(
                    f"<div style='margin-top:10px; font-size:0.8em; color:var(--text-secondary);'>Indexes: {idx_s}</div>")
            html.append("</div>")
        html.append("</details>")

    create_block("Tables", schema.tables, "sec-tables", True)
    create_block("Views", schema.views, "sec-views", False)
    if schema.triggers:
        html.append(
            f"<details open class='card' id='sec-triggers'><summary>Triggers <span class='counter-badge'>{len(schema.triggers)}</span></summary>")
        for t in sorted(schema.triggers, key=lambda x: x['name']):
            html.append(
                f"<div class='source-block' id='{t['name']}'><h3>⚡ {t['name']} <small>({t['table']})</small></h3><pre><code>{t['sql']}</code></pre></div>")
        html.append("</details>")
    html.append(
        f"<div class='card' id='sec-graph'><h2>ER Diagram</h2><div class='graph-container' id='vp'><pre class='mermaid'>{mmd}</pre></div></div></body></html>")
    with open("audit_report.html", "w", encoding="utf-8") as f:
        f.write("\n".join(html))
    print("Audit report generated: audit_report.html")


if __name__ == "__main__":
    parser = argparse.ArgumentParser();
    parser.add_argument("db");
    parser.add_argument("--html", action="store_true");
    parser.add_argument("--json", action="store_true");
    parser.add_argument("--mermaid", action="store_true")
    args = parser.parse_args();
    s = load_schema(args.db);
    resolve_views(s)
    if args.json:
        out = {"tables": {t: {c: o.to_dict() for c, o in cs.items()} for t, cs in s.tables.items()},
               "views": {v: {c: o.to_dict() for c, o in cs.items()} for v, cs in s.views.items()},
               "triggers": s.triggers, "warnings": s.warnings}
        with open("audit_report.json", "w", encoding="utf-8") as f: json.dump(out, f, indent=4)
    if args.mermaid:
        with open("audit_report.mmd", "w", encoding="utf-8") as f: f.write(generate_mermaid(s))
    if args.html: export_html(s)