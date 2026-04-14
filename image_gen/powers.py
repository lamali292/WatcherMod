from PIL import Image
import os
import math
import hashlib
import string
import shutil

# ============================================================
# CONFIG - edit these to use for your mod
# ============================================================
INPUT_DIR       = "powers"
OUT_POWERS      = "../Downfall/images/powers"
OUT_ATLASES     = "../Downfall/images/atlases"
ATLAS_SPRITES   = "power_atlas.sprites"
SPRITE_SPRITES  = "power_sprite_atlas.sprites"

BIG_SIZE        = 256
ATLAS_SIZE      = 64
SPRITE_SIZE     = 24

ATLAS_FILENAME        = "power_atlas.png"
SPRITE_ATLAS_FILENAME = "power_sprite_atlas.png"

# res:// paths for the tres files
ATLAS_RES_PATH        = "res://Downfall/images/atlases/power_atlas.png"
SPRITE_ATLAS_RES_PATH = "res://Downfall/images/atlases/power_sprite_atlas.png"

# folders whose contents go into sprite atlas only (not big/atlas)
SPRITE_ONLY_FOLDERS   = {"sts2"}

# suffix appended to non-sprite-only files
CUSTOM_SUFFIX         = "_power"
# ============================================================

OUT_TRES        = os.path.join(OUT_ATLASES, ATLAS_SPRITES)
OUT_TRES_SPRITE = os.path.join(OUT_ATLASES, SPRITE_SPRITES)

def deterministic_uid(name: str, length=7) -> str:
    return hashlib.md5(name.encode()).hexdigest()[:length]

def write_tres(path, atlas_res_path, x, y, size, name: str):
    content = f'''[gd_resource type="AtlasTexture" load_steps=2 format=3 uid="uid://{deterministic_uid(name)}"]
[ext_resource type="Texture2D" path="{atlas_res_path}" id="1"]
[resource]
atlas = ExtResource("1")
region = Rect2({x}, {y}, {size}, {size})
'''
    with open(path, "w") as f:
        f.write(content)

def clean_dir(folder, extensions):
    if not os.path.exists(folder):
        os.makedirs(folder)
        return
    for f in os.listdir(folder):
        if any(f.endswith(ext) for ext in extensions):
            os.remove(os.path.join(folder, f))

clean_dir(OUT_POWERS,      [".png", ".import"])
clean_dir(OUT_TRES,        [".tres"])
clean_dir(OUT_TRES_SPRITE, [".tres"])
os.makedirs(OUT_ATLASES, exist_ok=True)

# --- collect all images recursively ---
entries = []
for root, dirs, files in os.walk(INPUT_DIR):
    is_sprite_only = os.path.basename(root) in SPRITE_ONLY_FOLDERS
    for file in files:
        if file.lower().endswith(".png"):
            in_path = os.path.join(root, file)
            stem = os.path.splitext(file)[0]
            img = Image.open(in_path).convert("RGBA")
            big    = img.resize((BIG_SIZE,    BIG_SIZE),    Image.LANCZOS) if not is_sprite_only else None
            small  = img.resize((ATLAS_SIZE,  ATLAS_SIZE),  Image.LANCZOS) if not is_sprite_only else None
            sprite = img.resize((SPRITE_SIZE, SPRITE_SIZE), Image.LANCZOS)
            entries.append((stem, big, small, sprite, is_sprite_only))
            print("processed:", in_path, "→", stem, ("(sprite only)" if is_sprite_only else ""))

# --- build atlases ---
non_sprite_only = [(stem, big, small, sprite) for stem, big, small, sprite, is_sprite_only in entries if not is_sprite_only]
all_entries     = entries

n_atlas  = len(non_sprite_only)
n_sprite = len(all_entries)

cols_atlas  = math.ceil(math.sqrt(n_atlas))
rows_atlas  = math.ceil(n_atlas  / cols_atlas)
cols_sprite = math.ceil(math.sqrt(n_sprite))
rows_sprite = math.ceil(n_sprite / cols_sprite)

atlas        = Image.new("RGBA", (cols_atlas  * ATLAS_SIZE,  rows_atlas  * ATLAS_SIZE),  (0, 0, 0, 0))
sprite_atlas = Image.new("RGBA", (cols_sprite * SPRITE_SIZE, rows_sprite * SPRITE_SIZE), (0, 0, 0, 0))

for i, (stem, big, small, sprite) in enumerate(non_sprite_only):
    col = i % cols_atlas
    row = i // cols_atlas
    ax  = col * ATLAS_SIZE
    ay  = row * ATLAS_SIZE
    atlas.paste(small, (ax, ay))
    write_tres(os.path.join(OUT_TRES, f"{stem}{CUSTOM_SUFFIX}.tres"), ATLAS_RES_PATH, ax, ay, ATLAS_SIZE, f"{stem}_atlas")
    big.save(os.path.join(OUT_POWERS, f"{stem}{CUSTOM_SUFFIX}.png"))

for i, (stem, big, small, sprite, is_sprite_only) in enumerate(all_entries):
    col = i % cols_sprite
    row = i // cols_sprite
    sx  = col * SPRITE_SIZE
    sy  = row * SPRITE_SIZE
    sprite_atlas.paste(sprite, (sx, sy))
    filename = f"{stem}.tres" if is_sprite_only else f"{stem}{CUSTOM_SUFFIX}.tres"
    write_tres(os.path.join(OUT_TRES_SPRITE, filename), SPRITE_ATLAS_RES_PATH, sx, sy, SPRITE_SIZE, f"{stem}_sprite")

atlas.save(os.path.join(OUT_ATLASES, ATLAS_FILENAME))
sprite_atlas.save(os.path.join(OUT_ATLASES, SPRITE_ATLAS_FILENAME))

print(f"\n{ATLAS_FILENAME}: {cols_atlas}x{rows_atlas} grid ({n_atlas} images)")
print(f"{SPRITE_ATLAS_FILENAME}: {cols_sprite}x{rows_sprite} grid ({n_sprite} images)")
print("Done!")