using Godot;

namespace Watcher.Code.Stances;

[GlobalClass]
public partial class DivinityEyeSpawner : Node2D
{
    private const float SpawnInterval = 0.2f;

    private struct EyeData
    {
        public Sprite2D Sprite;
        public float Age;
        public float Lifetime;
        public float Scale;
        public Color BaseColor;
        public float BobSpeed;
        public float StartY;
    }

    private readonly System.Collections.Generic.List<EyeData> _eyes = new();
    private float _spawnTimer;
    private RandomNumberGenerator _rng = null!;
    private CanvasItemMaterial _mat = null!;
    private AtlasTexture[] _frames = null!;
    private bool _stopping;
    private float _s;

    public void StopSpawning() => _stopping = true;

    public override void _Ready()
    {
        _s = StanceVfx.VfxScale;
        Position *= _s;

        _rng = new RandomNumberGenerator();
        _rng.Randomize();
        _mat = new CanvasItemMaterial { BlendMode = CanvasItemMaterial.BlendModeEnum.Add };

        var strip = GD.Load<Texture2D>("res://Watcher/images/vfx/eye_anim.png");
        _frames = new AtlasTexture[7];
        for (int i = 0; i < 7; i++)
        {
            _frames[i] = new AtlasTexture();
            _frames[i].Atlas = strip;
            _frames[i].Region = new Rect2(i * 64, 0, 64, 64);
        }

        for (int i = 0; i < 3; i++)
        {
            float preAge = _rng.RandfRange(0f, 2.0f);
            SpawnEye(preAge);
        }
    }

    public override void _Process(double delta)
    {
        float dt = (float)delta;

        if (!_stopping)
        {
            _spawnTimer += dt;
            while (_spawnTimer >= SpawnInterval)
            {
                _spawnTimer -= SpawnInterval;
                SpawnEye(0f);
            }
        }
        else if (_eyes.Count == 0)
        {
            QueueFree();
            return;
        }

        for (int i = _eyes.Count - 1; i >= 0; i--)
        {
            var e = _eyes[i];
            e.Age += dt;

            if (e.Age >= e.Lifetime)
            {
                e.Sprite.QueueFree();
                _eyes.RemoveAt(i);
                continue;
            }

            float progress = e.Age / e.Lifetime;

            int frame = GetEyeFrame(progress);
            if (frame >= 0 && frame < _frames.Length)
                e.Sprite.Texture = _frames[frame];

            float bob = GetVerticalBob(progress, e.Scale);
            e.Sprite.Position = new Vector2(e.Sprite.Position.X, e.StartY + bob);

            float alpha;
            if (progress < 0.5f)
                alpha = progress * 2f;
            else
                alpha = (1f - progress) * 2f;
            alpha = alpha * alpha * (3f - 2f * alpha); // smoothstep

            e.Sprite.Modulate = new Color(e.BaseColor.R, e.BaseColor.G, e.BaseColor.B, alpha);
            _eyes[i] = e;
        }
    }

    private static int GetEyeFrame(float progress)
    {
        if (progress < 0.15f) return 0;
        if (progress < 0.20f) return 1;
        if (progress < 0.25f) return 2;
        if (progress < 0.30f) return 3;
        if (progress < 0.35f) return 4;
        if (progress < 0.40f) return 5;
        if (progress < 0.55f) return 6;
        if (progress < 0.62f) return 5;
        if (progress < 0.70f) return 4;
        if (progress < 0.75f) return 3;
        if (progress < 0.80f) return 2;
        if (progress < 0.85f) return 1;
        return 0;
    }

    private float GetVerticalBob(float progress, float scale)
    {
        int frame = GetEyeFrame(progress);
        float bobAmount = frame switch
        {
            0 => 12f,
            1 => 8f,
            2 => 4f,
            3 => 3f,
            _ => 0f,
        };
        return bobAmount * scale * _s;
    }

    private void SpawnEye(float initialAge)
    {
        var sprite = new Sprite2D();
        sprite.Texture = _frames[0];
        sprite.Material = _mat;

        float scale = _rng.RandfRange(1.25f, 1.88f) * _s;
        sprite.Scale = new Vector2(scale, scale);

        float px = _rng.RandfRange(-150f, 150f) * _s;
        float py = _rng.RandfRange(-175f, 75f) * _s;
        sprite.Position = new Vector2(px, py);

        float rot = _rng.RandfRange(6f, 12f);
        if (px > 0) rot = -rot;
        sprite.Rotation = Mathf.DegToRad(rot);

        float r = _rng.RandfRange(0.8f, 1.0f);
        float g = _rng.RandfRange(0.5f, 0.7f);
        float b = _rng.RandfRange(0.8f, 1.0f);
        var baseColor = new Color(r, g, b, 0f);
        sprite.Modulate = baseColor;

        sprite.ZIndex = _rng.Randf() < 0.5f ? -1 : 1;

        AddChild(sprite);

        float lifetime = scale / _s + 0.8f;

        _eyes.Add(new EyeData
        {
            Sprite = sprite,
            Age = initialAge,
            Lifetime = lifetime,
            Scale = scale,
            BaseColor = new Color(r, g, b, 1f),
            BobSpeed = 1f,
            StartY = py,
        });
    }
}
