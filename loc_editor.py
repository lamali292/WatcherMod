#!/usr/bin/env python3
"""
Run from your project root:
    python loc_editor.py

It scans Watcher/localization/<lang>/*.json automatically,
bundles everything and opens the editor in your browser.
"""

import json
import os
import webbrowser
import tempfile
from pathlib import Path

LOC_ROOT = Path("Watcher/localization")

# --- Collect all data ---
# Structure: { filename: { lang: { key: value } } }
files_data = {}

for lang_dir in sorted(LOC_ROOT.iterdir()):
    if not lang_dir.is_dir():
        continue
    lang = lang_dir.name
    for json_file in sorted(lang_dir.glob("*.json")):
        fname = json_file.name
        try:
            content = json.loads(json_file.read_text(encoding="utf-8-sig"))
        except Exception as e:
            print(f"Warning: could not parse {json_file}: {e}")
            continue
        if fname not in files_data:
            files_data[fname] = {}
        files_data[fname][lang] = content

if not files_data:
    print(f"No JSON files found under {LOC_ROOT}")
    exit(1)

print(f"Loaded {sum(len(v) for v in files_data.values())} lang/file combos across {len(files_data)} files")

# --- Build HTML ---
HTML = r"""<!DOCTYPE html>
<html lang="en">
<head>
<meta charset="UTF-8">
<title>Localization Editor</title>
<style>
  * { box-sizing: border-box; margin: 0; padding: 0; }
  body { font-family: 'Segoe UI', sans-serif; font-size: 13px; background: #1a1a1f; color: #ddd; height: 100vh; display: flex; flex-direction: column; }

  #toolbar {
    padding: 8px 12px; background: #111; border-bottom: 1px solid #333;
    display: flex; align-items: center; gap: 10px; flex-shrink: 0; flex-wrap: wrap;
  }
  #toolbar h1 { font-size: 14px; font-weight: 600; color: #fff; margin-right: 4px; }

  select, input[type=text] {
    padding: 4px 8px; background: #222; border: 1px solid #444; color: #ddd;
    border-radius: 4px; font-size: 12px; outline: none;
  }
  select:focus, input[type=text]:focus { border-color: #7a6fff; }
  #search { width: 200px; }

  button {
    padding: 4px 10px; background: #2a2a35; border: 1px solid #444; color: #ccc;
    border-radius: 4px; cursor: pointer; font-size: 12px;
  }
  button:hover { background: #35354a; border-color: #666; }
  button.primary { background: #5a4fcf; border-color: #7a6fff; color: #fff; }
  button.primary:hover { background: #6a5fdf; }

  .status { font-size: 11px; color: #555; margin-left: auto; }

  #table-area { flex: 1; overflow: hidden; display: flex; flex-direction: column; }
  .table-scroll { flex: 1; overflow: auto; }

  table { border-collapse: collapse; width: 100%; min-width: max-content; }
  thead { position: sticky; top: 0; z-index: 10; }
  thead th {
    background: #111; padding: 8px 10px; text-align: left; font-size: 12px;
    border-bottom: 2px solid #333; border-right: 1px solid #2a2a2a; white-space: nowrap; color: #aaa;
  }
  thead th:first-child { position: sticky; left: 0; z-index: 11; background: #111; min-width: 300px; max-width: 300px; width: 300px; }
  .lang-label { display: flex; align-items: center; gap: 6px; }
  .dot { width: 7px; height: 7px; border-radius: 50%; flex-shrink: 0; }

  tbody tr { border-bottom: 1px solid #232328; }
  tbody tr:hover td { background: #1e1e28 !important; }
  tbody td { border-right: 1px solid #232328; vertical-align: top; padding: 0; }
  tbody td:first-child {
    position: sticky; left: 0; background: #1a1a1f; padding: 6px 10px;
    color: #666; font-size: 11px; font-family: monospace;
    min-width: 300px; max-width: 300px; width: 300px;
    overflow: hidden; text-overflow: ellipsis; white-space: nowrap;
  }
  tbody tr:hover td:first-child { background: #1e1e28; }

  textarea {
    width: 100%; min-width: 260px; padding: 6px 10px; background: transparent; border: none;
    color: #ddd; font-family: 'Segoe UI', sans-serif; font-size: 12px;
    resize: none; outline: none; line-height: 1.5; overflow: hidden; min-height: 30px; display: block;
  }
  textarea:focus { background: #1e1e30; }
  textarea.empty { color: #666; background: rgba(192,57,43,0.07); font-style: italic; }
  textarea.modified { background: rgba(90,79,207,0.1); }
</style>
</head>
<body>

<div id="toolbar">
  <h1>🌐 Loc Editor</h1>
  <label style="font-size:12px;color:#888;">File:</label>
  <select id="file-select" onchange="switchFile()"></select>
  <input type="text" id="search" placeholder="Filter keys or values..." oninput="filterRows()">
  <button class="primary" onclick="exportAll()">Export JSON</button>
  <span class="status" id="status"></span>
</div>

<div id="table-area">
  <div class="table-scroll">
    <table id="main-table">
      <thead><tr id="header-row"></tr></thead>
      <tbody id="table-body"></tbody>
    </table>
  </div>
</div>

<script>
const ALL_DATA = __ALL_DATA__;

const COLORS = ['#7a6fff','#f7c46a','#6af79a','#f7906a','#6ac8f7','#f76aaa','#c06af7'];
let modified = {};
let currentFile = null;
let langs = [];
let allKeys = [];

// Init file selector
const sel = document.getElementById('file-select');
Object.keys(ALL_DATA).sort().forEach(fname => {
  const opt = document.createElement('option');
  opt.value = fname; opt.textContent = fname;
  sel.appendChild(opt);
});

switchFile();

function switchFile() {
  currentFile = document.getElementById('file-select').value;
  const fileData = ALL_DATA[currentFile];
  langs = Object.keys(fileData).sort().map((name, i) => ({ name, color: COLORS[i % COLORS.length] }));

  // Collect all keys preserving order
  const keySet = new Set();
  langs.forEach(l => Object.keys(fileData[l.name]).forEach(k => keySet.add(k)));
  allKeys = [...keySet];

  renderHeader();
  renderRows(allKeys);
  updateStatus();
}

function renderHeader() {
  const hr = document.getElementById('header-row');
  hr.innerHTML = '<th>Key</th>';
  langs.forEach(lang => {
    const th = document.createElement('th');
    th.innerHTML = `<div class="lang-label"><span class="dot" style="background:${lang.color}"></span>${lang.name}</div>`;
    hr.appendChild(th);
  });
}

function renderRows(keys) {
  const fileData = ALL_DATA[currentFile];
  const modFile = modified[currentFile] || {};
  const tbody = document.getElementById('table-body');
  tbody.innerHTML = '';

  keys.forEach(key => {
    const tr = document.createElement('tr');
    const tdKey = document.createElement('td');
    tdKey.textContent = key; tdKey.title = key;
    tr.appendChild(tdKey);

    langs.forEach(lang => {
      const td = document.createElement('td');
      const original = fileData[lang.name]?.[key] ?? '';
      const stored = modFile[lang.name]?.[key];
      const val = stored !== undefined ? stored : original;

      const ta = document.createElement('textarea');
      ta.value = val; ta.rows = 1;
      if (!val) ta.classList.add('empty');
      if (stored !== undefined) ta.classList.add('modified');

      ta.addEventListener('input', () => {
        autoResize(ta);
        if (!modified[currentFile]) modified[currentFile] = {};
        if (!modified[currentFile][lang.name]) modified[currentFile][lang.name] = {};
        const orig = fileData[lang.name]?.[key] ?? '';
        if (ta.value !== orig) {
          modified[currentFile][lang.name][key] = ta.value;
          ta.classList.add('modified'); ta.classList.remove('empty');
        } else {
          delete modified[currentFile][lang.name][key];
          ta.classList.remove('modified');
          if (!ta.value) ta.classList.add('empty');
        }
        updateStatus();
      });
      ta.addEventListener('focus', () => autoResize(ta));
      ta.addEventListener('blur', () => { ta.style.height = ''; });
      td.appendChild(ta);
      tr.appendChild(td);
    });
    tbody.appendChild(tr);
  });
}

function autoResize(ta) {
  ta.style.height = 'auto';
  ta.style.height = ta.scrollHeight + 'px';
}

function filterRows() {
  const q = document.getElementById('search').value.toLowerCase();
  const fileData = ALL_DATA[currentFile];
  const filtered = !q ? allKeys : allKeys.filter(k => {
    if (k.toLowerCase().includes(q)) return true;
    return langs.some(l => (fileData[l.name]?.[k] ?? '').toLowerCase().includes(q));
  });
  renderRows(filtered);
}

function updateStatus() {
  const modFile = modified[currentFile] || {};
  const modCount = Object.values(modFile).reduce((a, b) => a + Object.keys(b).length, 0);
  document.getElementById('status').textContent =
    `${allKeys.length} keys · ${langs.length} langs${modCount ? ` · ${modCount} modified` : ''}`;
}

function exportAll() {
  const fileData = ALL_DATA[currentFile];
  const modFile = modified[currentFile] || {};
  langs.forEach(lang => {
    const out = {};
    allKeys.forEach(key => {
      const val = modFile[lang.name]?.[key] !== undefined
        ? modFile[lang.name][key]
        : fileData[lang.name]?.[key];
      if (val !== undefined) out[key] = val;
    });
    const blob = new Blob([JSON.stringify(out, null, 2)], { type: 'application/json' });
    const a = document.createElement('a');
    a.href = URL.createObjectURL(blob);
    a.download = lang.name + '_' + currentFile;
    a.click();
  });
}
</script>
</body>
</html>
"""

html = HTML.replace("__ALL_DATA__", json.dumps(files_data, ensure_ascii=False, indent=2))

# Write to temp file and open
tmp = tempfile.NamedTemporaryFile(delete=False, suffix=".html", mode="w", encoding="utf-8")
tmp.write(html)
tmp.close()
webbrowser.open(f"file://{tmp.name}")
print(f"Opened editor: {tmp.name}")