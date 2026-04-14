using Godot;
using MegaCrit.Sts2.Core.Assets;

namespace Watcher.Code.Stances.Vfx;

[GlobalClass]
public partial class WrathActivationBurst : Node2D
{
    private const int LineCount = 10;
    private const float TotalDuration = 1.1f;
    private const float CircleRadius = 44f;
    private float _elapsed;

    private BurstLine[] _lines = null!;

    public override void _Ready()
    {
        var s = StanceVfx.VfxScale;
        Position *= s;

        var rng = new RandomNumberGenerator();
        rng.Randomize();

        _lines = new BurstLine[LineCount];
        var mat = new CanvasItemMaterial { BlendMode = CanvasItemMaterial.BlendModeEnum.Add };
        var texture = PreloadManager.Cache.GetAsset<Texture2D>("res://Watcher/images/vfx/strike_line.png");

        for (var i = 0; i < LineCount; i++)
        {
            var sprite = new Sprite2D();
            sprite.Texture = texture;
            sprite.Material = mat;

            var g = rng.RandfRange(0.15f, 0.35f);
            sprite.Modulate = new Color(1.0f, g, 0.05f, 0f);

            var angle = i / (float)LineCount * Mathf.Tau + rng.RandfRange(-0.2f, 0.2f);
            var radius = (CircleRadius + rng.RandfRange(-10f, 10f)) * s;
            sprite.Position = new Vector2(
                Mathf.Cos(angle) * radius,
                Mathf.Sin(angle) * radius * 0.3f
            );

            sprite.Rotation = Mathf.DegToRad(90f);

            var thinScale = rng.RandfRange(0.8f, 1.2f) * s;
            var tallScale = rng.RandfRange(12.5f, 18.75f) * s;
            sprite.Scale = new Vector2(0.5f, thinScale);

            if (i == 0) sprite.Position = new Vector2(0, 0);

            AddChild(sprite);

            var delay = rng.RandfRange(0f, 0.1f);
            _lines[i] = new BurstLine(sprite, delay, 0.9f, 0.4f, thinScale, tallScale);
        }
    }

    public override void _Process(double delta)
    {
        _elapsed += (float)delta;

        if (_elapsed > TotalDuration)
        {
            QueueFree();
            return;
        }

        for (var i = 0; i < _lines.Length; i++)
        {
            ref readonly var line = ref _lines[i];
            var t = _elapsed - line.Delay;

            if (t < 0)
            {
                line.Sprite.Modulate = new Color(line.Sprite.Modulate, 0f);
                continue;
            }

            var progress = t / line.Duration;
            if (progress > 1f)
            {
                line.Sprite.Modulate = new Color(line.Sprite.Modulate, 0f);
                continue;
            }

            var scaleProgress = Mathf.Min(progress / 0.5f, 1f);
            var currentHeight = Mathf.Lerp(0.3f, line.EndScaleY, scaleProgress);
            line.Sprite.Scale = new Vector2(currentHeight, line.StartScaleX);

            float alpha;
            if (progress < 0.15f)
            {
                var p = progress / 0.15f;
                alpha = p * p * p * line.TargetAlpha;
            }
            else
            {
                var p = (progress - 0.15f) / 0.85f;
                alpha = line.TargetAlpha * (1f - p);
            }

            line.Sprite.Modulate = new Color(line.Sprite.Modulate, alpha);
        }
    }

    private readonly struct BurstLine(
        Sprite2D sprite,
        float delay,
        float duration,
        float targetAlpha,
        float startScaleX,
        float endScaleY)
    {
        public readonly Sprite2D Sprite = sprite;
        public readonly float Delay = delay;
        public readonly float Duration = duration;
        public readonly float TargetAlpha = targetAlpha;
        public readonly float StartScaleX = startScaleX;
        public readonly float EndScaleY = endScaleY;
    }
}