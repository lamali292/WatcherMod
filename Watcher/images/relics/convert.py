import os
import sys

def remove_watcher_prefix(folder):
    for root, _, files in os.walk(folder):
        for file in files:
            if file.endswith(".png") and file.startswith("watcher-"):
                old = os.path.join(root, file)
                new = os.path.join(root, file[len("watcher-"):])
                os.rename(old, new)
                print(f"Renamed: {file} → {file[len('watcher-'):]}")

if __name__ == "__main__":
    folder = sys.argv[1] if len(sys.argv) > 1 else "."
    remove_watcher_prefix(folder)