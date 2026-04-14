from PIL import Image
import numpy as np
from scipy.ndimage import gaussian_filter
import os
import math
import random
import string

def process_image(path, radius=5, sigma=1.2):
    # 1. Open original 64x64 and crop to the 44x44 core
    img_full = Image.open(path).convert("RGBA")
    crop_margin = 8
    img_core = img_full.crop((crop_margin, crop_margin, 64 - crop_margin, 64 - crop_margin))
    
    # 2. Main Potion: Smooth Upscale 44x44 -> 80x80
    # LANCZOS provides the highest quality smoothing for upscaling small assets
    image_80 = img_core.resize((80, 80), Image.LANCZOS)

    # 3. Outline: High-res buffer for ultra-smooth blur
    # We upscale to 256 using BILINEAR here to ensure the alpha map is already smooth
    img_upscaled = img_core.resize((256, 256), Image.BILINEAR)
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

    # Apply Gaussian filter to the dilated mask for the "glow" effect
    outline_alpha = gaussian_filter(dilated.astype(np.float32), sigma=sigma)
    outline_alpha = np.clip(outline_alpha, 0, 255).astype(np.uint8)

    outline_full = np.zeros((256, 256, 4), dtype=np.uint8)
    outline_full[..., 0:3] = 255 # White
    outline_full[..., 3] = outline_alpha
    
    # Resize the glow to the final 80x80 slot
    outline_80 = Image.fromarray(outline_full, "RGBA").resize((80, 80), Image.LANCZOS)

    return outline_80, image_80

def random_uid(length=7):
    return ''.join(random.choices(string.ascii_lowercase + string.digits, k=length))

def write_tres(path, atlas_res_path, x, y, size):
    content = f'''[gd_resource type="AtlasTexture" load_steps=2 format=3 uid="uid://{random_uid()}"]
[ext_resource type="Texture2D" path="{atlas_res_path}" id="1"]
[resource]
atlas = ExtResource("1")
region = Rect2({x}, {y}, {size}, {size})
'''
    with open(path, "w") as f:
        f.write(content)

# --- Config ---
input_dir = "old"
os.makedirs("atlas", exist_ok=True)
os.makedirs("tres", exist_ok=True)

IMG_SIZE = 80
ATLAS_PATH = "res://Watcher/images/potions/atlas/watcher_atlas.png"
OUTLINE_PATH = "res://Watcher/images/potions/atlas/watcher_outline_atlas.png"

# --- Process ---
entries = []
for file in sorted(os.listdir(input_dir)):
    if file.lower().endswith(".png"):
        outline_80, image_80 = process_image(os.path.join(input_dir, file))
        entries.append((os.path.splitext(file)[0], outline_80, image_80))

if not entries:
    print("No images found.")
    exit()

# --- Build Atlases ---
n = len(entries)
cols = math.ceil(math.sqrt(n))
rows = math.ceil(n / cols)
atlas_w, atlas_h = cols * IMG_SIZE, rows * IMG_SIZE

main_atlas = Image.new("RGBA", (atlas_w, atlas_h), (0, 0, 0, 0))
outline_atlas = Image.new("RGBA", (atlas_w, atlas_h), (0, 0, 0, 0))

for i, (stem, outline_80, image_80) in enumerate(entries):
    x = (i % cols) * IMG_SIZE
    y = (i // cols) * IMG_SIZE
    
    main_atlas.paste(image_80, (x, y))
    outline_atlas.paste(outline_80, (x, y))

    write_tres(os.path.join("tres", f"{stem}.tres"), ATLAS_PATH, x, y, IMG_SIZE)
    write_tres(os.path.join("tres", f"{stem}_outline.tres"), OUTLINE_PATH, x, y, IMG_SIZE)

main_atlas.save(os.path.join("atlas", "watcher_atlas.png"))
outline_atlas.save(os.path.join("atlas", "watcher_outline_atlas.png"))

print(f"Done! {n} potions upscaled smoothly to 80x80.")