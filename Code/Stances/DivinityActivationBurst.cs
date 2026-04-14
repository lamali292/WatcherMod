using Godot;

namespace Watcher.Code.Stances;

[GlobalClass]
public partial class DivinityActivationBurst : Node2D
{
    private const int SparkCount = 20;
    private const float TotalDuration = 1.2f;

    private struct SparkInfo
    {
        public Sprite2D Sprite;
        public float Delay;
        public float Duration;
        public float StartX;
        public float StartY;
        public float ScaleStart;
        public Color BaseColor;
    }

    private SparkInfo[] _sparks = null!;
    private float _elapsed;

    public override void _Ready()
    {
        float s = StanceVfx.VfxScale;
        Position *= s;

        var rng = new RandomNumberGenerator();
        rng.Randomize();

        _sparks = new SparkInfo[SparkCount];
        var mat = new CanvasItemMaterial { BlendMode = CanvasItemMaterial.BlendModeEnum.Add };
        var tex = GD.Load<Texture2D>("res://Watcher/images/vfx/glow_spark.png");

        for (int i = 0; i < SparkCount; i++)
        {
            var sprite = new Sprite2D();
            sprite.Texture = tex;
            sprite.Material = mat;

            float r = rng.RandfRange(0.8f, 1.0f);
            float g = rng.RandfRange(0.4f, 0.7f);
            float b = rng.RandfRange(0.7f, 1.0f);
            sprite.Modulate = new Color(r, g, b, 0f);

            float angle = rng.RandfRange(0f, Mathf.Tau);
            float dist = rng.RandfRange(625f, 1000f) * s;
            float sx = Mathf.Cos(angle) * dist;
            float sy = Mathf.Sin(angle) * dist * 0.5f;
            sprite.Position = new Vector2(sx, sy);

            sprite.Rotation = Mathf.Atan2(-sy, -sx) + Mathf.Pi * 0.5f;

            float scaleStart = rng.RandfRange(1.88f, 3.75f) * s;
            sprite.Scale = new Vector2(scaleStart * 0.5f, scaleStart * 1.5f);

            AddChild(sprite);

            _sparks[i] = new SparkInfo
            {
                Sprite = sprite,
                Delay = rng.RandfRange(0f, 0.25f),
                Duration = rng.RandfRange(0.35f, 0.55f),
                StartX = sx,
                StartY = sy,
                ScaleStart = scaleStart,
                BaseColor = new Color(r, g, b, 1f),
            };
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

        for (int i = 0; i < _sparks.Length; i++)
        {
            ref readonly var sp = ref _sparks[i];
            float t = _elapsed - sp.Delay;

            if (t < 0)
            {
                sp.Sprite.Modulate = new Color(sp.BaseColor, 0f);
                continue;
            }

            float progress = Mathf.Clamp(t / sp.Duration, 0f, 1f);

            float inward = progress * progress;
            float cx = Mathf.Lerp(sp.StartX, 0f, inward);
            float cy = Mathf.Lerp(sp.StartY, 0f, inward);
            sp.Sprite.Position = new Vector2(cx, cy);

            float scale = sp.ScaleStart * (1f - progress * 0.7f);
            sp.Sprite.Scale = new Vector2(scale * 0.5f, scale * 1.5f);

            float alpha;
            if (progress < 0.15f)
                alpha = progress / 0.15f;
            else if (progress < 0.5f)
                alpha = 1f;
            else
                alpha = (1f - progress) / 0.5f;
            alpha = Mathf.Clamp(alpha * 0.65f, 0f, 1f);

            sp.Sprite.Modulate = new Color(sp.BaseColor, alpha);
        }
    }
}
