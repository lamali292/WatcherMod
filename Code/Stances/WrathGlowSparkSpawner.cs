using Godot;

namespace Watcher.Code.Stances;

[GlobalClass]
public partial class WrathGlowSparkSpawner : Node2D
{
    private const float SpawnInterval = 0.05f;
    private const float SparkLifetime = 1.4f;
    private const float SpawnRadius = 205f;
    private const float StartScaleX = 0.62f;
    private const float StartScaleY = 1.02f;
    private const float EndScaleY = 4.1f;

    private struct SparkData
    {
        public Sprite2D Sprite;
        public float Age;
        public float Lifetime;
        public float FinalScaleY;
        public float BaseScaleX;
        public Color BaseColor;
    }

    private readonly System.Collections.Generic.List<SparkData> _sparks = new();
    private float _spawnTimer;
    private RandomNumberGenerator _rng = null!;
    private CanvasItemMaterial _mat = null!;
    private Texture2D _texture = null!;
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
        _texture = GD.Load<Texture2D>("res://Watcher/images/vfx/glow_spark.png");

        for (int i = 0; i < 20; i++)
        {
            float preAge = _rng.RandfRange(0f, SparkLifetime);
            SpawnSpark(preAge);
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
                SpawnSpark(0f);
            }
        }
        else if (_sparks.Count == 0)
        {
            QueueFree();
            return;
        }

        for (int i = _sparks.Count - 1; i >= 0; i--)
        {
            var s = _sparks[i];
            s.Age += dt;

            if (s.Age >= s.Lifetime)
            {
                s.Sprite.QueueFree();
                _sparks.RemoveAt(i);
                continue;
            }

            float progress = s.Age / s.Lifetime;

            float scaleY = Mathf.Lerp(StartScaleY * _s, s.FinalScaleY, progress);
            s.Sprite.Scale = new Vector2(s.BaseScaleX, scaleY);

            float alpha;
            if (progress < 0.25f)
                alpha = progress / 0.25f;
            else if (progress < 0.6f)
                alpha = 1.0f;
            else
                alpha = 1.0f - (progress - 0.6f) / 0.4f;

            s.Sprite.Modulate = new Color(s.BaseColor.R, s.BaseColor.G, s.BaseColor.B, alpha);
            _sparks[i] = s;
        }
    }

    private void SpawnSpark(float initialAge)
    {
        var sprite = new Sprite2D();
        sprite.Texture = _texture;
        sprite.Material = _mat;

        sprite.Offset = new Vector2(0, -31 * _s);

        float angle = _rng.RandfRange(0f, Mathf.Tau);
        float dist = _rng.RandfRange(0f, SpawnRadius * _s);
        sprite.Position = new Vector2(
            Mathf.Cos(angle) * dist,
            Mathf.Sin(angle) * dist
        );

        sprite.Rotation = Mathf.DegToRad(_rng.RandfRange(-8f, 8f));

        float r = _rng.RandfRange(0.25f, 0.35f);
        float g = _rng.RandfRange(0f, 0.03f);
        var baseColor = new Color(r, g, 0.02f, 0f);
        sprite.Modulate = baseColor;

        float baseScaleX = _rng.RandfRange(0.52f, 0.72f) * _s;
        float finalScaleY = _rng.RandfRange(EndScaleY * 0.8f, EndScaleY * 1.2f) * _s;
        sprite.Scale = new Vector2(baseScaleX, StartScaleY * _s);

        bool behind = _rng.Randf() < 0.5f;
        if (behind)
        {
            sprite.ZAsRelative = false;
            sprite.ZIndex = -1;
        }

        AddChild(sprite);

        _sparks.Add(new SparkData
        {
            Sprite = sprite,
            Age = initialAge,
            Lifetime = _rng.RandfRange(SparkLifetime * 0.8f, SparkLifetime * 1.2f),
            FinalScaleY = finalScaleY,
            BaseScaleX = baseScaleX,
            BaseColor = new Color(r, g, 0.02f, 1f),
        });
    }
}
