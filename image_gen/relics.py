from PIL import Image
import numpy as np
from scipy.ndimage import gaussian_filter
import os
import math
import hashlib
import string

# ============================================================
# CONFIG - edit these to use for your mod
# ============================================================
INPUT_DIR       = "relics"
OUT_RELICS      = "../Watcher/images/relics"
OUT_ATLASES     = "../Watcher/images/atlases"
ATLAS_SPRITES   = "relic_atlas.sprites"

IMG_SIZE        = 85
INSET           = 4
REGION_SIZE     = 85

ATLAS_FILENAME         = "relic_atlas.png"
OUTLINE_ATLAS_FILENAME = "relic_outline_atlas.png"

ATLAS_RES_PATH         = "res://Watcher/images/atlases/relic_atlas.png"
OUTLINE_ATLAS_RES_PATH = "res://Watcher/images/atlases/relic_outline_atlas.png"

# process_image crop/outline settings
CROP_BOX        = (56, 56, 200, 200)
OUTLINE_RADIUS  = 10
OUTLINE_SIGMA   = 0.5
# ============================================================

OUT_TRES = os.path.join(OUT_ATLASES, ATLAS_SPRITES)

def clean_dir(folder, extensions):
    if not os.path.exists(folder):
        os.makedirs(folder)
        return
    for f in os.listdir(folder):
        if any(f.endswith(ext) for ext in extensions):
            os.remove(os.path.join(folder, f))

clean_dir(OUT_RELICS,  [".png", ".import"])
clean_dir(OUT_TRES,    [".tres"])
os.makedirs(OUT_ATLASES, exist_ok=True)

def deterministic_uid(name: str, length=7) -> str:
    return hashlib.md5(name.encode()).hexdigest()[:length]

def write_tres(path, atlas_res_path, x, y, size, name):
    content = f'''[gd_resource type="AtlasTexture" load_steps=2 format=3 uid="uid://{deterministic_uid(name)}"]
[ext_resource type="Texture2D" path="{atlas_res_path}" id="1"]
[resource]
atlas = ExtResource("1")
region = Rect2({x}, {y}, {size}, {size})
'''
    with open(path, "w") as f:
        f.write(content)

def process_image(path, crop_box=CROP_BOX, radius=OUTLINE_RADIUS, sigma=OUTLINE_SIGMA):
    img = Image.open(path).convert("RGBA")
    img_cropped = img.crop(crop_box)
    img_upscaled = img_cropped.resize((256, 256), Image.LANCZOS)
    alpha = np.array(img_upscaled.split()[3])
    size = radius * 2 + 1
    y, x = np.ogrid[-radius:radius+1, -radius:radius+1]
    kernel = (x*x + y*y) <= radius*radius
    padded = np.pad(alpha, radius)
    dilated = np.zeros_like(alpha)
    for dy in range(size):
        for dx in range(size):
            if kernel[dy, dx]:
                dilated = np.maximum(dilated, padded[dy:dy+alpha.shape[0], dx:dx+alpha.shape[1]])
    outline_alpha = gaussian_filter(dilated.astype(np.float32), sigma=sigma)
    outline_alpha = np.clip(outline_alpha, 0, 255).astype(np.uint8)
    outline = np.zeros((alpha.shape[0], alpha.shape[1], 4), dtype=np.uint8)
    outline[..., 0] = 255
    outline[..., 1] = 255
    outline[..., 2] = 255
    outline[..., 3] = outline_alpha
    image_outline = Image.fromarray(outline, "RGBA")
    black_outline = np.zeros((alpha.shape[0], alpha.shape[1], 4), dtype=np.uint8)
    black_outline[..., 3] = (outline_alpha * 0.5).astype(np.uint8)
    black_result = Image.fromarray(black_outline, "RGBA")
    big = Image.alpha_composite(black_result, img_upscaled)
    outline_downscaled = image_outline.resize((IMG_SIZE, IMG_SIZE), Image.LANCZOS)
    image_downscaled = img_cropped.resize((IMG_SIZE, IMG_SIZE), Image.LANCZOS)
    return big, outline_downscaled, image_downscaled

# --- collect all images ---
entries = []
for file in os.listdir(INPUT_DIR):
    if file.lower().endswith(".png"):
        in_path = os.path.join(INPUT_DIR, file)
        stem = os.path.splitext(file)[0]
        big, outline_downscaled, image_downscaled = process_image(in_path)
        entries.append((stem, big, outline_downscaled, image_downscaled))
        print("processed:", file, "→", stem)

# --- build atlases ---
n = len(entries)
cols = math.ceil(math.sqrt(n))
rows = math.ceil(n / cols)
atlas_w = cols * IMG_SIZE
atlas_h = rows * IMG_SIZE

atlas         = Image.new("RGBA", (atlas_w, atlas_h), (0, 0, 0, 0))
outline_atlas = Image.new("RGBA", (atlas_w, atlas_h), (0, 0, 0, 0))

for i, (stem, big, outline_downscaled, image_downscaled) in enumerate(entries):
    col = i % cols
    row = i // cols
    x = col * IMG_SIZE
    y = row * IMG_SIZE
    atlas.paste(image_downscaled, (x, y))
    outline_atlas.paste(outline_downscaled, (x, y))
    big.save(os.path.join(OUT_RELICS, f"{stem}.png"))
    write_tres(os.path.join(OUT_TRES, f"{stem}.tres"),         ATLAS_RES_PATH,         x + INSET, y + INSET, REGION_SIZE, stem)
    write_tres(os.path.join(OUT_TRES, f"{stem}_outline.tres"), OUTLINE_ATLAS_RES_PATH, x + INSET, y + INSET, REGION_SIZE, f"{stem}_outline")

atlas.save(os.path.join(OUT_ATLASES, ATLAS_FILENAME))
outline_atlas.save(os.path.join(OUT_ATLASES, OUTLINE_ATLAS_FILENAME))

print(f"\nAtlases saved: {atlas_w}x{atlas_h}px, {n} images ({cols}x{rows} grid)")
print("Done!")