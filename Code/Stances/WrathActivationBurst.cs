using Godot;

namespace Watcher.Code.Stances;

[GlobalClass]
public partial class WrathActivationBurst : Node2D
{
    private const int LineCount = 10;
    private const float TotalDuration = 1.1f;
    private const float CircleRadius = 44f;

    private readonly struct BurstLine
    {
        public readonly Sprite2D Sprite;
        public readonly float Delay;
        public readonly float Duration;
        public readonly float TargetAlpha;
        public readonly float StartScaleX;
        public readonly float EndScaleY;

        public BurstLine(Sprite2D sprite, float delay, float duration, float targetAlpha,
                         float startScaleX, float endScaleY)
        {
            Sprite = sprite;
            Delay = delay;
            Duration = duration;
            TargetAlpha = targetAlpha;
            StartScaleX = startScaleX;
            EndScaleY = endScaleY;
        }
    }

    private BurstLine[] _lines = null!;
    private float _elapsed;

    public override void _Ready()
    {
        float s = StanceVfx.VfxScale;
        Position *= s;

        var rng = new RandomNumberGenerator();
        rng.Randomize();

        _lines = new BurstLine[LineCount];
        var mat = new CanvasItemMaterial { BlendMode = CanvasItemMaterial.BlendModeEnum.Add };

        for (int i = 0; i < LineCount; i++)
        {
            var sprite = new Sprite2D();
            sprite.Texture = GD.Load<Texture2D>("res://Watcher/images/vfx/strike_line.png");
            sprite.Material = mat;

            float g = rng.RandfRange(0.15f, 0.35f);
            sprite.Modulate = new Color(1.0f, g, 0.05f, 0f);

            float angle = (i / (float)LineCount) * Mathf.Tau + rng.RandfRange(-0.2f, 0.2f);
            float radius = (CircleRadius + rng.RandfRange(-10f, 10f)) * s;
            sprite.Position = new Vector2(
                Mathf.Cos(angle) * radius,
                Mathf.Sin(angle) * radius * 0.3f
            );

            sprite.Rotation = Mathf.DegToRad(90f);

            float thinScale = rng.RandfRange(0.8f, 1.2f) * s;
            float tallScale = rng.RandfRange(12.5f, 18.75f) * s;
            sprite.Scale = new Vector2(0.5f, thinScale);

            if (i == 0)
            {
                sprite.Position = new Vector2(0, 0);
            }

            AddChild(sprite);

            float delay = rng.RandfRange(0f, 0.1f);
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

        for (int i = 0; i < _lines.Length; i++)
        {
            ref readonly var line = ref _lines[i];
            float t = _elapsed - line.Delay;

            if (t < 0)
            {
                line.Sprite.Modulate = new Color(line.Sprite.Modulate, 0f);
                continue;
            }

            float progress = t / line.Duration;
            if (progress > 1f)
            {
                line.Sprite.Modulate = new Color(line.Sprite.Modulate, 0f);
                continue;
            }

            float scaleProgress = Mathf.Min(progress / 0.5f, 1f);
            float currentHeight = Mathf.Lerp(0.3f, line.EndScaleY, scaleProgress);
            line.Sprite.Scale = new Vector2(currentHeight, line.StartScaleX);

            float alpha;
            if (progress < 0.15f)
            {
                float p = progress / 0.15f;
                alpha = p * p * p * line.TargetAlpha;
            }
            else
            {
                float p = (progress - 0.15f) / 0.85f;
                alpha = line.TargetAlpha * (1f - p);
            }

            line.Sprite.Modulate = new Color(line.Sprite.Modulate, alpha);
        }
    }
}
