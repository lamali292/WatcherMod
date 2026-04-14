using Godot;
using MegaCrit.Sts2.Core.Helpers;

namespace Watcher.Code.Stances;

[GlobalClass]
public partial class ScreenFlashEffect : CanvasLayer
{
    private const float FadeInDuration = 0.12f;
    private const float FlashDuration = 0.7f;

    private TextureRect _tex = null!;
    private float _elapsed;
    private Color _flashColor;

    public static void Play(Color color)
    {
        if (NonInteractiveMode.IsActive) return;

        var flash = new ScreenFlashEffect();
        flash._flashColor = color;
        flash.Layer = 100;

        var tree = Engine.GetMainLoop() as SceneTree;
        tree?.Root.AddChild(flash);
    }

    public override void _Ready()
    {
        _tex = new TextureRect();
        _tex.Texture = GD.Load<Texture2D>("res://Watcher/images/vfx/screenflash.png");
        _tex.Material = new CanvasItemMaterial { BlendMode = CanvasItemMaterial.BlendModeEnum.Add };
        _tex.StretchMode = TextureRect.StretchModeEnum.Scale;
        _tex.AnchorRight = 1;
        _tex.AnchorBottom = 1;
        _tex.Modulate = _flashColor;
        AddChild(_tex);
    }

    public override void _Process(double delta)
    {
        _elapsed += (float)delta;
        if (_elapsed >= FlashDuration)
        {
            QueueFree();
            return;
        }

        float alpha;
        if (_elapsed < FadeInDuration)
        {
            alpha = _elapsed / FadeInDuration;
        }
        else
        {
            float fadeOut = (_elapsed - FadeInDuration) / (FlashDuration - FadeInDuration);
            alpha = 1f - fadeOut * fadeOut;
        }
        _tex.Modulate = new Color(_flashColor.R, _flashColor.G, _flashColor.B, alpha);
    }
}
