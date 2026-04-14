import re
import sys

def add_watcher_prefix(filepath):
    with open(filepath, 'r', encoding='utf-8') as f:
        content = f.read()

    # Match JSON keys that don't already have the WATCHER- prefix
    result = re.sub(r'"(?!WATCHER-)([A-Z_]+\.)', r'"WATCHER-\1', content)

    with open(filepath, 'w', encoding='utf-8') as f:
        f.write(result)

    print(f"Done: {filepath}")

if __name__ == "__main__":
    filepath = sys.argv[1] if len(sys.argv) > 1 else "cards.json"
    add_watcher_prefix(filepath)