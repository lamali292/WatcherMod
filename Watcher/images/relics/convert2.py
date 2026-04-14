import os
import numpy as np
from PIL import Image

radius = 12

script_dir = os.path.dirname(os.path.abspath(__file__))
input_dir = os.path.join(script_dir, "old")
output_dir = script_dir

size = radius * 2 + 1
y, x = np.ogrid[-radius:radius+1, -radius:radius+1]
kernel = (x*x + y*y) <= radius*radius


def outline_image(in_path, out_path):
    img = Image.open(in_path).convert("RGBA")
    alpha = np.array(img.split()[3])

    padded = np.pad(alpha, radius)
    dilated = np.zeros_like(alpha)

    for dy in range(size):
        for dx in range(size):
            if kernel[dy, dx]:
                dilated = np.maximum(
                    dilated,
                    padded[dy:dy+alpha.shape[0], dx:dx+alpha.shape[1]]
                )

    outline_mask = np.clip(dilated - alpha, 0, 255)
    outline_alpha = (outline_mask * 0.5).astype(np.uint8)

    outline = np.zeros((alpha.shape[0], alpha.shape[1], 4), dtype=np.uint8)
    outline[..., 3] = outline_alpha

    outline_img = Image.fromarray(outline, "RGBA")
    result = Image.alpha_composite(outline_img, img)

    result.save(out_path)


for file in os.listdir(input_dir):
    if file.lower().endswith(".png"):
        in_path = os.path.join(input_dir, file)

        name = file if file.startswith("watcher-") else f"watcher-{file}"
        out_path = os.path.join(output_dir, name)

        outline_image(in_path, out_path)
        print("processed:", file, "→", name)

print("done.")