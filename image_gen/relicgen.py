import json
import hashlib
import math
import threading
import numpy as np
import tkinter as tk
from dataclasses import dataclass
from pathlib import Path
from tkinter import ttk, filedialog
from typing import Tuple, Dict
from PIL import Image, ImageTk
from scipy.ndimage import gaussian_filter, binary_dilation

CONFIG_FILE = Path("relic_config.json")

# ============================================================
# TYPE-SAFE PATH CONFIGURATION
# ============================================================

@dataclass(frozen=True)
class PipelinePaths:
    input_dir: Path
    godot_root: Path
    res_base: str

    @property
    def relative_res_path(self) -> Path:
        return Path(self.res_base.replace("res://", "").strip("/"))

    @property
    def out_relics(self) -> Path:
        return self.godot_root / self.relative_res_path / "relics"

    @property
    def out_atlases(self) -> Path:
        return self.godot_root / self.relative_res_path / "atlases"
        
    def ensure_directories(self) -> None:
        self.out_relics.mkdir(parents=True, exist_ok=True)
        self.out_atlases.mkdir(parents=True, exist_ok=True)

# ============================================================
# CORE PROCESSING LOGIC
# ============================================================

def get_deterministic_uid(name: str) -> str:
    return hashlib.md5(name.encode()).hexdigest()[:12]

def generate_godot_tres(path: Path, atlas_res_path: str, x: int, y: int, size: int) -> None:
    uid = get_deterministic_uid(path.stem)
    content = (
        f'[gd_resource type="AtlasTexture" load_steps=2 format=3 uid="uid://{uid}"]\n'
        f'[ext_resource type="Texture2D" path="{atlas_res_path}" id="1"]\n'
        f'[resource]\n'
        f'atlas = ExtResource("1")\n'
        f'region = Rect2({x}, {y}, {size}, {size})\n'
    )
    path.write_text(content)

def process_image_robust(
    path: Path, crop_inset: int, radius: int, sigma: float, big_size: int, atlas_size: int, resample_mode: int
) -> Tuple[Image.Image, Image.Image, Image.Image]:
    with Image.open(path) as img:
        img = img.convert("RGBA")
        w, h = img.size
        box = (-crop_inset, -crop_inset, w + crop_inset, h + crop_inset)
        
        pad_l, pad_t = max(0, -box[0]), max(0, -box[1])
        pad_r, pad_b = max(0, box[2] - w), max(0, box[3] - h)
        
        if any(p > 0 for p in (pad_l, pad_t, pad_r, pad_b)):
            expanded = Image.new("RGBA", (w + pad_l + pad_r, h + pad_t + pad_b), (0,0,0,0))
            expanded.paste(img, (pad_l, pad_t))
            img_cropped = expanded
        else:
            img_cropped = img.crop(box)

        img_upscaled = img_cropped.resize((big_size, big_size), resample_mode)
        alpha = np.array(img_upscaled.split()[3])

        y_grid, x_grid = np.ogrid[-radius:radius+1, -radius:radius+1]
        mask = x_grid*x_grid + y_grid*y_grid <= radius*radius
        dilated = binary_dilation(alpha > 0, structure=mask).astype(np.float32) * 255
        
        outline_alpha = gaussian_filter(dilated, sigma=sigma).astype(np.uint8)
        
        white_arr = np.full((big_size, big_size, 4), 255, dtype=np.uint8)
        white_arr[..., 3] = outline_alpha
        img_outline_white = Image.fromarray(white_arr, "RGBA")

        shadow_arr = np.zeros((big_size, big_size, 4), dtype=np.uint8)
        shadow_arr[..., 3] = (outline_alpha * 0.5).astype(np.uint8)
        img_big = Image.alpha_composite(Image.fromarray(shadow_arr, "RGBA"), img_upscaled)

        outline_ds = img_outline_white.resize((atlas_size, atlas_size), resample_mode)
        atlas_ds = img_cropped.resize((atlas_size, atlas_size), resample_mode)

        return img_big, outline_ds, atlas_ds

# ============================================================
# UI SYSTEM
# ============================================================

class RelicApp(tk.Tk):
    def __init__(self):
        super().__init__()
        self.title("Relic Pipeline Pro")
        self.geometry("1300x900")
        self.configure(bg="#121212")
        
        self.current_file: Path | None = None
        self.tk_cache: Dict[str, ImageTk.PhotoImage] = {}
        self.preview_debounce = None
        self._is_updating_ui = False
        
        self.resample_map = {
            "Lanczos": Image.LANCZOS, "Nearest": Image.NEAREST, 
            "Bilinear": Image.BILINEAR, "Bicubic": Image.BICUBIC, "Box": Image.BOX
        }

        self._load_config()
        self._setup_styles()
        self._build_layout()
        self._refresh_files()

    # --- CONFIG MANAGER ---
    def _load_config(self) -> None:
        if CONFIG_FILE.exists():
            try:
                with open(CONFIG_FILE, 'r') as f:
                    self.config = json.load(f)
            except Exception:
                self._default_config()
        else:
            self._default_config()
            
    def _default_config(self):
        self.config = {
            "paths": {"input_dir": "relics", "godot_root": "../", "res_base": "res://MOD_ID/images"},
            "globals": {"inset": 0},
            "per_image": {}
        }

    def _save_config(self) -> None:
        with open(CONFIG_FILE, 'w') as f:
            json.dump(self.config, f, indent=4)

    def _setup_styles(self) -> None:
        s = ttk.Style()
        s.theme_use("clam")
        dark_bg = "#1e1e1e"
        accent = "#3d5afe"
        field_bg = "#2a2a2a"
        fg = "#e0e0e0"

        s.configure(".", background=dark_bg, foreground=fg, font=("Segoe UI", 10))
        s.configure("TFrame", background=dark_bg)
        s.configure("Header.TLabel", font=("Segoe UI Bold", 11), foreground=accent)
        s.configure("Card.TFrame", background="#252525", relief="flat")
        s.configure("Action.TButton", background=accent, foreground="white", padding=10)
        s.map("Action.TButton", background=[("active", "#536dfe")])

        # Fix Entry
        s.configure("TEntry",
                    fieldbackground=field_bg, foreground=fg,
                    insertcolor=fg, bordercolor="#444", lightcolor=field_bg,
                    darkcolor=field_bg, padding=4)
        s.map("TEntry",
              fieldbackground=[("focus", "#333"), ("!focus", field_bg)],
              bordercolor=[("focus", accent), ("!focus", "#444")])

        # Fix Combobox
        s.configure("TCombobox",
                    fieldbackground=field_bg, foreground=fg,
                    background=field_bg, selectbackground=field_bg,
                    selectforeground=fg, bordercolor="#444",
                    arrowcolor=fg, padding=4)
        s.map("TCombobox",
              fieldbackground=[("readonly", field_bg)],
              foreground=[("readonly", fg)],
              selectbackground=[("readonly", field_bg)],
              selectforeground=[("readonly", fg)])

        # Fix dropdown list (the popup part of Combobox)
        self.option_add("*TCombobox*Listbox.background", field_bg)
        self.option_add("*TCombobox*Listbox.foreground", fg)
        self.option_add("*TCombobox*Listbox.selectBackground", accent)
        self.option_add("*TCombobox*Listbox.selectForeground", "white")

        # Fix Scrollbar
        s.configure("TScrollbar", background="#333", troughcolor=dark_bg,
                    bordercolor=dark_bg, arrowcolor="#888")

    def _build_layout(self) -> None:
        sidebar = ttk.Frame(self, width=380, style="TFrame")
        sidebar.pack(side=tk.LEFT, fill=tk.Y, padx=20, pady=20)
        sidebar.pack_propagate(False)

        # 1. Project Paths (Auto-saving)
        self._create_section_label(sidebar, "PROJECT PATHS")
        self.path_in = self._create_path_row(sidebar, "Input Folder", "input_dir")
        self.path_root = self._create_path_row(sidebar, "Godot Root", "godot_root")
        
        ttk.Label(sidebar, text="Res Base Path").pack(anchor="w", pady=(5,0))
        self.res_base = tk.StringVar(value=self.config["paths"].get("res_base", "res://assets/images"))
        self.res_base.trace_add("write", lambda *a: self._save_path_var("res_base", self.res_base))
        ttk.Entry(sidebar, textvariable=self.res_base).pack(fill=tk.X, pady=5)

        # 2. Source Files
        self._create_section_label(sidebar, "SOURCE FILES")
        fb_frame = ttk.Frame(sidebar)
        fb_frame.pack(fill=tk.BOTH, expand=1, pady=5)
        self.file_list = tk.Listbox(fb_frame, bg="#181818", fg="#bbb", bd=0, highlightthickness=1)
        self.file_list.pack(side=tk.LEFT, fill=tk.BOTH, expand=True)
        self.file_list.bind("<<ListboxSelect>>", self._on_file_select)
        
        scroller = ttk.Scrollbar(fb_frame, orient="vertical", command=self.file_list.yview)
        scroller.pack(side=tk.RIGHT, fill=tk.Y)
        self.file_list.config(yscrollcommand=scroller.set)

        # 3. Parameters
        self._create_section_label(sidebar, "IMAGE PARAMETERS")
        self.params: Dict[str, tk.IntVar] = {}
        
        # Build Custom Crop Inset Row
        self._build_inset_row(sidebar)
        
        self._add_slider(sidebar, "Big Size", "big_size", 64, 512, 256)
        self._add_slider(sidebar, "Atlas Size", "atlas_size", 16, 256, 85)
        self._add_slider(sidebar, "Outline Radius", "radius", 1, 30, 8)
        self._add_slider(sidebar, "Blur Sigma", "sigma", 1, 50, 5) 

        ttk.Label(sidebar, text="Resampling Method").pack(anchor="w", pady=(10, 0))
        self.resample_var = tk.StringVar(value="Lanczos")
        resample_menu = ttk.Combobox(sidebar, textvariable=self.resample_var, values=list(self.resample_map.keys()), state="readonly")
        resample_menu.pack(fill=tk.X, pady=5)
        resample_menu.bind("<<ComboboxSelected>>", lambda e: self._queue_preview())

        # Footer Actions
        self.btn_run = ttk.Button(sidebar, text="GENERATE ALL ASSETS", style="Action.TButton", command=self._start_generation)
        self.btn_run.pack(fill=tk.X, pady=(20, 10))
        
        self.progress = ttk.Progressbar(sidebar, mode="determinate")
        self.progress.pack(fill=tk.X)
        self.status = ttk.Label(sidebar, text="Ready", wraplength=350)
        self.status.pack(anchor="w", pady=5)

        # Previews
        main_view = ttk.Frame(self)
        main_view.pack(side=tk.LEFT, fill=tk.BOTH, expand=True, padx=20, pady=20)
        grid = ttk.Frame(main_view)
        grid.pack(fill=tk.BOTH, expand=True)
        
        self.canvases: Dict[str, tk.Canvas] = {}
        self.canvas_labels: Dict[str, ttk.Label] = {}
        panels = [("Big Preview", "big"), ("Original Source", "orig"), ("Atlas Texture", "atlas"), ("Outline Texture", "outline")]
        
        for i, (title, key) in enumerate(panels):
            r, c = i // 2, i % 2
            f = ttk.Frame(grid, style="Card.TFrame")
            f.grid(row=r, column=c, padx=10, pady=10, sticky="nsew")
            lbl = ttk.Label(f, text=title, background="#252525", font=("Segoe UI Bold", 9))
            lbl.pack(pady=5)
            self.canvas_labels[key] = lbl
            
            canvas = tk.Canvas(f, bg="#1a1a1a", highlightthickness=0, bd=0)
            canvas.pack(fill=tk.BOTH, expand=True, padx=2, pady=2)
            self.canvases[key] = canvas
            
        grid.columnconfigure((0,1), weight=1)
        grid.rowconfigure((0,1), weight=1)

    # --- UI Builders ---
    def _create_section_label(self, parent: tk.Widget, text: str) -> None:
        ttk.Label(parent, text=text, style="Header.TLabel").pack(anchor="w", pady=(15, 5))

    def _save_path_var(self, key: str, var: tk.StringVar) -> None:
        self.config.setdefault("paths", {})[key] = var.get()
        self._save_config()

    def _create_path_row(self, parent: tk.Widget, label: str, config_key: str) -> tk.StringVar:
        ttk.Label(parent, text=label).pack(anchor="w")
        row = ttk.Frame(parent)
        row.pack(fill=tk.X, pady=2)
        
        var = tk.StringVar(value=self.config.setdefault("paths", {}).get(config_key, ""))
        var.trace_add("write", lambda *a, k=config_key, v=var: self._save_path_var(k, v))
        
        ttk.Entry(row, textvariable=var).pack(side=tk.LEFT, fill=tk.X, expand=True)
        ttk.Button(row, text="...", width=3, command=lambda: self._pick_dir(var)).pack(side=tk.LEFT, padx=2)
        return var

    def _build_inset_row(self, parent: tk.Widget):
        row = ttk.Frame(parent)
        row.pack(fill=tk.X, pady=1)

        self.inset_label_var = tk.StringVar(value="Crop Inset")
        ttk.Label(row, textvariable=self.inset_label_var, width=14).pack(side=tk.LEFT)

        global_val = self.config.setdefault("globals", {}).get("inset", 0)
        self.params['inset'] = tk.IntVar(value=global_val)

        self.inset_val_lbl = ttk.Label(row, text=str(global_val), width=4)
        self.inset_val_lbl.pack(side=tk.RIGHT)

        btn_style = dict(bg="#2a2a2a", fg="#888", relief="flat", bd=0,
                         activebackground="#333", activeforeground="#ccc",
                         font=("Segoe UI", 9), cursor="hand2", padx=4)

        tk.Button(row, text="↺", command=self._reset_local_inset,
                  **btn_style).pack(side=tk.RIGHT, padx=(2, 0))
        tk.Button(row, text="◆", command=self._set_as_global_inset,
                  **{**btn_style, "fg": "#3d5afe"}).pack(side=tk.RIGHT, padx=(2, 0))

        ttk.Scale(row, from_=-100, to=100, variable=self.params['inset'],
                  command=self._on_inset_drag).pack(side=tk.LEFT, fill=tk.X, expand=True)

    def _set_as_global_inset(self) -> None:
        v = self.params['inset'].get()
        self.config.setdefault("globals", {})["inset"] = v

        # Remove local override — this file now matches global
        if self.current_file:
            self.config.get("per_image", {}).pop(self.current_file.name, None)
            self.inset_label_var.set("Crop Inset")

        self._save_config()
        self._refresh_files()

    def _add_slider(self, parent: tk.Widget, label: str, key: str, lo: int, hi: int, default: int) -> None:
        row = ttk.Frame(parent)
        row.pack(fill=tk.X, pady=1)
        ttk.Label(row, text=label, width=14).pack(side=tk.LEFT)
        var = tk.IntVar(value=default)
        self.params[key] = var
        val_lbl = ttk.Label(row, text=str(default), width=4)
        val_lbl.pack(side=tk.RIGHT)
        
        def update_val(e, k=key, l=val_lbl, v=var):
            display = f"{v.get()/10:.1f}" if k == "sigma" else str(v.get())
            l.config(text=display)
            self._queue_preview()

        ttk.Scale(row, from_=lo, to=hi, variable=var, command=update_val).pack(side=tk.LEFT, fill=tk.X, expand=True)

    # --- Interaction Logic ---
    def _pick_dir(self, var: tk.StringVar) -> None:
        d = filedialog.askdirectory()
        if d: 
            var.set(d)
            self._refresh_files()

    def _get_paths(self) -> PipelinePaths:
        return PipelinePaths(
            input_dir=Path(self.path_in.get()), godot_root=Path(self.path_root.get()), res_base=self.res_base.get()
        )

    def _refresh_files(self) -> None:
        self.file_list.delete(0, tk.END)
        paths = self._get_paths()
        if paths.input_dir.exists():
            per_image = self.config.get("per_image", {})
            for f in sorted(paths.input_dir.glob("*.png")):
                self.file_list.insert(tk.END, f.name)
                has_local = f.name in per_image and "inset" in per_image[f.name]
                self.file_list.itemconfig(tk.END, fg="#ffd54f" if has_local else "#bbb")

    def _on_file_select(self, event) -> None:
        sel = self.file_list.curselection()
        if sel:
            self._is_updating_ui = True
            paths = self._get_paths()
            self.current_file = paths.input_dir / self.file_list.get(sel[0])

            # Apply local config if it exists, otherwise fall back to global
            local_conf = self.config.setdefault("per_image", {}).get(self.current_file.name, {})
            if "inset" in local_conf:
                self.params['inset'].set(local_conf["inset"])
                self.inset_label_var.set("Crop Inset (*)")
            else:
                self.params['inset'].set(self.config.setdefault("globals", {}).get("inset", 0))
                self.inset_label_var.set("Crop Inset")

            self.inset_val_lbl.config(text=str(self.params['inset'].get()))
            self._is_updating_ui = False
            self._queue_preview()

    def _on_inset_drag(self, val) -> None:
        v = self.params['inset'].get()
        self.inset_val_lbl.config(text=str(v))

        if self._is_updating_ui: return

        if self.current_file:
            self.config.setdefault("per_image", {}).setdefault(self.current_file.name, {})["inset"] = v
            self.inset_label_var.set("Crop Inset (*)")

        self._save_config()
        self._refresh_files()
        self._queue_preview()

    def _reset_local_inset(self) -> None:
        if self.current_file and self.current_file.name in self.config.get("per_image", {}):
            del self.config["per_image"][self.current_file.name]
            self._save_config()

            self._is_updating_ui = True
            self.params['inset'].set(self.config.setdefault("globals", {}).get("inset", 0))
            self.inset_label_var.set("Crop Inset")
            self.inset_val_lbl.config(text=str(self.params['inset'].get()))
            self._is_updating_ui = False

            self._refresh_files()   # <-- added
            self._queue_preview()

    def _queue_preview(self) -> None:
        if self.preview_debounce: 
            self.after_cancel(self.preview_debounce)
        self.preview_debounce = self.after(100, self._render_preview)

    def _render_preview(self) -> None:
        if not self.current_file: return
        
        resample_mode = self.resample_map.get(self.resample_var.get(), Image.LANCZOS)
        self.canvas_labels['big'].config(text=f"Big ({self.params['big_size'].get()}px)")
        self.canvas_labels['atlas'].config(text=f"Atlas ({self.params['atlas_size'].get()}px)")
        
        try:
            big, out, atl = process_image_robust(
                self.current_file, self.params['inset'].get(),
                self.params['radius'].get(), self.params['sigma'].get()/10,
                self.params['big_size'].get(), self.params['atlas_size'].get(), resample_mode
            )
            
            self._display_on_canvas("big", big)
            self._display_on_canvas("atlas", atl)
            self._display_on_canvas("outline", out)
            with Image.open(self.current_file) as orig:
                self._display_on_canvas("orig", orig.convert("RGBA"))
        except Exception as e:
            self.status.config(text=f"Preview error: {e}")

    def _display_on_canvas(self, key: str, img: Image.Image) -> None:
        canvas = self.canvases[key]
        canvas.update()
        cw, ch = canvas.winfo_width(), canvas.winfo_height()
        canvas.delete("all")
        
        step = 10
        for i in range(0, cw, step*2):
            for j in range(0, ch, step*2):
                canvas.create_rectangle(i, j, i+step, j+step, fill="#222", outline="")
                canvas.create_rectangle(i+step, j+step, i+step*2, j+step*2, fill="#222", outline="")

        img_w, img_h = img.size
        ratio = min((cw-10)/img_w, (ch-10)/img_h)
        zoom_w, zoom_h = int(img_w * ratio), int(img_h * ratio)
        
        display_img = img.resize((zoom_w, zoom_h), Image.NEAREST if ratio > 1 else Image.LANCZOS)
        photo = ImageTk.PhotoImage(display_img)
        canvas.create_image(cw//2, ch//2, image=photo)
        
        x0, y0 = cw//2 - zoom_w//2, ch//2 - zoom_h//2
        canvas.create_rectangle(x0, y0, x0 + zoom_w, y0 + zoom_h, outline="#ff4444", width=1)
        self.tk_cache[key] = photo

    # --- Worker Thread ---
    def _start_generation(self) -> None:
        self.btn_run.state(["disabled"])
        paths = self._get_paths()
        threading.Thread(target=self._proc_task, args=(paths,), daemon=True).start()

    def _proc_task(self, paths: PipelinePaths) -> None:
        try:
            paths.ensure_directories()
            resample_mode = self.resample_map.get(self.resample_var.get(), Image.LANCZOS)
            files = list(paths.input_dir.glob("*.png"))
            self.progress["maximum"] = len(files)
            
            atlas_size = self.params['atlas_size'].get()
            atlas_items = []
            global_inset = self.config.setdefault("globals", {}).get("inset", 0)
            
            for i, fpath in enumerate(files):
                self.after(0, lambda x=i, f=fpath: (self.status.config(text=f"Processing: {f.name}"), self.progress.step(1)))
                
                # Resolve active inset (Local > Global)
                local_conf = self.config.setdefault("per_image", {}).get(fpath.name, {})
                active_inset = local_conf.get("inset", global_inset)
                
                res = process_image_robust(
                    fpath, active_inset, self.params['radius'].get(), 
                    self.params['sigma'].get()/10, self.params['big_size'].get(), 
                    atlas_size, resample_mode
                )
                
                big, out_ds, atl_ds = res
                big.save(paths.out_relics / fpath.name)
                atlas_items.append((fpath.stem, atl_ds, out_ds))

            count = len(atlas_items)
            cols = math.ceil(math.sqrt(count))
            rows = math.ceil(count / cols)
            
            full_atlas = Image.new("RGBA", (cols*atlas_size, rows*atlas_size), (0,0,0,0))
            full_outline = Image.new("RGBA", (cols*atlas_size, rows*atlas_size), (0,0,0,0))
            
            for i, (name, img, out) in enumerate(atlas_items):
                x, y = (i % cols) * atlas_size, (i // cols) * atlas_size
                full_atlas.paste(img, (x, y))
                full_outline.paste(out, (x, y))
                
                tres_path = paths.out_atlases / f"{name}.tres"
                generate_godot_tres(tres_path, f"{paths.res_base}/atlases/relic_atlas.png", x, y, atlas_size)

            full_atlas.save(paths.out_atlases / "relic_atlas.png")
            full_outline.save(paths.out_atlases / "relic_outline_atlas.png")
            
            self.after(0, lambda: self.status.config(text=f"Success! Exported {count} relics."))
        except Exception as e:
            self.after(0, lambda: self.status.config(text=f"Error: {e}"))
        finally:
            self.after(0, lambda: self.btn_run.state(["!disabled"]))

if __name__ == "__main__":
    app = RelicApp()
    app.mainloop()