#!/usr/bin/env python3
"""
Run from your project root:
    python loc_editor.py
"""

import json
import webbrowser
import threading
from pathlib import Path
from http.server import HTTPServer, BaseHTTPRequestHandler
from urllib.parse import urlparse, parse_qs

LOC_ROOT = Path("")
SETTINGS_PATH = Path("editor") / "loc_editor_settings.json"
ENGLISH_LANG = "eng"   # 3-letter code for the fallback/source language

# ── Load data ──────────────────────────────────────────────────────────────

files_data = {}
file_paths = {}
all_langs = set()

for lang_dir in sorted(LOC_ROOT.iterdir()):
    if not lang_dir.is_dir() or len(lang_dir.name) != 3:
        continue
    lang = lang_dir.name
    all_langs.add(lang)
    for json_file in sorted(lang_dir.glob("*.json")):
        fname = json_file.name
        try:
            content = json.loads(json_file.read_text(encoding="utf-8-sig"))
        except Exception as e:
            print(f"Warning: could not parse {json_file}: {e}")
            continue
        if fname not in files_data:
            files_data[fname] = {}
            file_paths[fname] = {}
        files_data[fname][lang] = content
        file_paths[fname][lang] = str(json_file)

all_langs = sorted(all_langs)

for fname in files_data:
    for lang in all_langs:
        if lang not in files_data[fname]:
            files_data[fname][lang] = {}

if not files_data:
    print(f"No JSON files found under {LOC_ROOT}")
    exit(1)

print(f"Loaded {len(files_data)} files, {len(all_langs)} languages: {', '.join(all_langs)}")
print(f"English/fallback language: {ENGLISH_LANG}")


# ── Settings ───────────────────────────────────────────────────────────────

def load_settings():
    if SETTINGS_PATH.exists():
        try:
            return json.loads(SETTINGS_PATH.read_text(encoding="utf-8"))
        except Exception:
            pass
    return {"activeLangs": all_langs, "colWidths": {}, "keyColWidth": 280}


def save_settings(s):
    SETTINGS_PATH.write_text(json.dumps(s, ensure_ascii=False, indent=2), encoding="utf-8")


# ── Helpers ────────────────────────────────────────────────────────────────

def write_lang_file(file, lang):
    """Write files_data[file][lang] to disk."""
    if file in file_paths and lang in file_paths[file]:
        p = Path(file_paths[file][lang])
    else:
        ld = LOC_ROOT / lang
        ld.mkdir(parents=True, exist_ok=True)
        p = ld / file
        file_paths.setdefault(file, {})[lang] = str(p)
        print(f"Creating: {p}")
    p.write_text(
        json.dumps(files_data[file][lang], ensure_ascii=False, indent=2),
        encoding="utf-8"
    )


# ── HTML (loaded from sibling file) ───────────────────────────────────────

HTML_PATH = Path("editor") / "loc_editor.html"
HTML_TEMPLATE = HTML_PATH.read_text(encoding="utf-8")


def build_html():
    s = load_settings()
    payload = {
        "files_data": files_data,
        "all_langs": all_langs,
        "english_lang": ENGLISH_LANG,
        "settings": s,
    }
    return HTML_TEMPLATE.replace("__PAYLOAD__", json.dumps(payload, ensure_ascii=False))


# ── HTTP server ────────────────────────────────────────────────────────────

class Handler(BaseHTTPRequestHandler):
    def log_message(self, format, *args):
        pass

    def send(self, code, body=b"", content_type="application/json"):
        self.send_response(code)
        self.send_header("Content-Type", content_type)
        self.send_header("Cache-Control", "no-store")
        self.end_headers()
        if isinstance(body, str):
            body = body.encode("utf-8")
        self.wfile.write(body)

    def do_GET(self):
        path = urlparse(self.path).path
        if path == "/":
            self.send(200, build_html(), "text/html; charset=utf-8")
        elif path == "/settings":
            self.send(200, json.dumps(load_settings(), ensure_ascii=False))
        else:
            self.send(404)

    def read_json_body(self):
        length = int(self.headers.get("Content-Length", 0))
        return json.loads(self.rfile.read(length))

    def do_POST(self):
        path = urlparse(self.path).path

        if path == "/save":
            try:
                payload = self.read_json_body()
                file = payload["file"]
                lang = payload["lang"]
                key = payload["key"]
                value = payload["value"]

                d = files_data[file].setdefault(lang, {})

                if not value and lang != ENGLISH_LANG:
                    # Remove key from non-English files — English will be used as fallback
                    if key in d:
                        del d[key]
                        write_lang_file(file, lang)
                        print(f"Removed empty key '{key}' from {lang}/{file}")
                else:
                    d[key] = value
                    write_lang_file(file, lang)

                self.send(204)
            except Exception as e:
                self.send(500, str(e), "text/plain")

        elif path == "/rename":
            try:
                payload = self.read_json_body()
                file = payload["file"]
                old_key = payload["old_key"]
                new_key = payload["new_key"].strip()

                if not new_key or new_key == old_key:
                    self.send(204)
                    return

                all_keys = set()
                for lang in all_langs:
                    all_keys.update(files_data[file].get(lang, {}).keys())

                if new_key in all_keys:
                    self.send(409, f'Key "{new_key}" already exists', "text/plain")
                    return

                for lang in all_langs:
                    d = files_data[file].get(lang, {})
                    if old_key in d:
                        d[new_key] = d.pop(old_key)

                for lang in all_langs:
                    if files_data[file].get(lang):
                        write_lang_file(file, lang)

                print(f"Renamed {old_key} -> {new_key}")
                self.send(204)
            except Exception as e:
                self.send(500, str(e), "text/plain")

        elif path == "/add_key":
            try:
                payload = self.read_json_body()
                file = payload["file"]
                new_key = payload["key"].strip()

                if not new_key:
                    self.send(400, "Key cannot be empty", "text/plain")
                    return

                all_keys = set()
                for lang in all_langs:
                    all_keys.update(files_data[file].get(lang, {}).keys())

                if new_key in all_keys:
                    self.send(409, f'Key "{new_key}" already exists', "text/plain")
                    return

                # Only write the empty key to English — other langs get it on first save
                files_data[file].setdefault(ENGLISH_LANG, {})[new_key] = ""
                write_lang_file(file, ENGLISH_LANG)

                # Keep in-memory dicts in sync (but don't write other lang files)
                for lang in all_langs:
                    if lang != ENGLISH_LANG:
                        files_data[file].setdefault(lang, {})

                print(f"Added key: '{new_key}' (written to {ENGLISH_LANG} only)")
                self.send(204)
            except Exception as e:
                self.send(500, str(e), "text/plain")

        elif path == "/settings":
            try:
                payload = self.read_json_body()
                s = load_settings()
                s.update(payload)
                save_settings(s)
                self.send(204)
            except Exception as e:
                self.send(500, str(e), "text/plain")

        else:
            self.send(404)


PORT = 7842
server = HTTPServer(("localhost", PORT), Handler)
print(f"Running at http://localhost:{PORT}")
print(f"Settings: {SETTINGS_PATH}")

threading.Timer(0.8, lambda: webbrowser.open(f"http://localhost:{PORT}")).start()

print("Press Ctrl+C to stop.")
try:
    server.serve_forever()
except KeyboardInterrupt:
    print("\nStopped.")