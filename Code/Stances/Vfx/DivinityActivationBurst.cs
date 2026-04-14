using Godot;

namespace Watcher.Code.Stances.Vfx;

[GlobalClass]
public partial class DivinityActivationBurst : Node2D
{
    private const int SparkCount = 20;
    private const float TotalDuration = 1.2f;
    private float _elapsed;

    private SparkInfo[] _sparks = null!;

    public override void _Ready()
    {
        const float s = StanceVfx.VfxScale;
        Position *= s;

        var rng = new RandomNumberGenerator();
        rng.Randomize();

        _sparks = new SparkInfo[SparkCount];
        var mat = new CanvasItemMaterial { BlendMode = CanvasItemMaterial.BlendModeEnum.Add };
        var tex = GD.Load<Texture2D>("res://Watcher/images/vfx/glow_spark.png");

        for (var i = 0; i < SparkCount; i++)
        {
            var sprite = new Sprite2D();
            sprite.Texture = tex;
            sprite.Material = mat;

            var r = rng.RandfRange(0.8f, 1.0f);
            var g = rng.RandfRange(0.4f, 0.7f);
            var b = rng.RandfRange(0.7f, 1.0f);
            sprite.Modulate = new Color(r, g, b, 0f);

            var angle = rng.RandfRange(0f, Mathf.Tau);
            var dist = rng.RandfRange(625f, 1000f) * s;
            var sx = Mathf.Cos(angle) * dist;
            var sy = Mathf.Sin(angle) * dist * 0.5f;
            sprite.Position = new Vector2(sx, sy);

            sprite.Rotation = Mathf.Atan2(-sy, -sx) + Mathf.Pi * 0.5f;

            var scaleStart = rng.RandfRange(1.88f, 3.75f) * s;
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
                BaseColor = new Color(r, g, b)
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

        foreach (var t1 in _sparks)
        {
            ref readonly var sp = ref t1;
            var t = _elapsed - sp.Delay;

            if (t < 0)
            {
                sp.Sprite.Modulate = new Color(sp.BaseColor, 0f);
                continue;
            }

            var progress = Mathf.Clamp(t / sp.Duration, 0f, 1f);

            var inward = progress * progress;
            var cx = Mathf.Lerp(sp.StartX, 0f, inward);
            var cy = Mathf.Lerp(sp.StartY, 0f, inward);
            sp.Sprite.Position = new Vector2(cx, cy);

            var scale = sp.ScaleStart * (1f - progress * 0.7f);
            sp.Sprite.Scale = new Vector2(scale * 0.5f, scale * 1.5f);

            var alpha = progress switch
            {
                < 0.15f => progress / 0.15f,
                < 0.5f => 1f,
                _ => (1f - progress) / 0.5f
            };
            alpha = Mathf.Clamp(alpha * 0.65f, 0f, 1f);

            sp.Sprite.Modulate = new Color(sp.BaseColor, alpha);
        }
    }

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
}