using Godot;
using MegaCrit.Sts2.Core.Assets;

namespace Watcher.Code.Stances.Vfx;

[GlobalClass]
public partial class CalmFrostStreakSpawner : Node2D
{
    private const float SpawnInterval = 0.04f;
    private const float MinLifetime = 1.1f;
    private const float MaxLifetime = 1.7f;

    private readonly List<StreakData> _streaks = [];
    private CanvasItemMaterial _mat = null!;
    private RandomNumberGenerator _rng = null!;
    private float _s;
    private float _spawnTimer;
    private bool _stopping;
    private Texture2D _texture = null!;

    public override void _Ready()
    {
        _s = StanceVfx.VfxScale;
        Position *= _s;

        _rng = new RandomNumberGenerator();
        _rng.Randomize();
        _mat = new CanvasItemMaterial { BlendMode = CanvasItemMaterial.BlendModeEnum.Add };
        _texture = PreloadManager.Cache.GetAsset<Texture2D>("res://Watcher/images/vfx/frost_streak.png");

        for (var i = 0; i < 15; i++)
        {
            var preAge = _rng.RandfRange(0f, MaxLifetime);
            SpawnStreak(preAge);
        }
    }

    public void StopSpawning()
    {
        _stopping = true;
    }

    public override void _Process(double delta)
    {
        var dt = (float)delta;

        if (!_stopping)
        {
            _spawnTimer += dt;
            while (_spawnTimer >= SpawnInterval)
            {
                _spawnTimer -= SpawnInterval;
                SpawnStreak(0f);
            }
        }
        else if (_streaks.Count == 0)
        {
            QueueFree();
            return;
        }

        for (var i = _streaks.Count - 1; i >= 0; i--)
        {
            var s = _streaks[i];
            s.Age += dt;

            if (s.Age >= s.Lifetime)
            {
                s.Sprite.QueueFree();
                _streaks.RemoveAt(i);
                continue;
            }

            s.VelocityX += s.AccelX * dt;
            s.VelocityY += s.AccelY * dt;

            s.VelocityX = Mathf.Min(s.VelocityX, -20f * _s);
            s.VelocityY = Mathf.Max(s.VelocityY, 0f);

            s.Sprite.Position += new Vector2(s.VelocityX * dt, s.VelocityY * dt);

            s.Sprite.Rotation = Mathf.Atan2(s.VelocityY, s.VelocityX) + Mathf.Pi * 0.5f;

            var progress = s.Age / s.Lifetime;

            float yScaleMult;
            if (progress < 0.4f)
                yScaleMult = 0.5f + progress * 2.5f;
            else if (progress < 0.7f)
                yScaleMult = 1.5f;
            else
                yScaleMult = 1.5f - (progress - 0.7f) * 3.3f;
            yScaleMult = Mathf.Max(yScaleMult, 0.2f);
            s.Sprite.Scale = new Vector2(s.BaseScale * 0.375f, s.BaseScale * 1.76f * yScaleMult);

            float alpha;
            if (progress < 0.3f)
                alpha = progress / 0.3f;
            else if (progress < 0.8f)
                alpha = 1.0f;
            else
                alpha = (1f - progress) / 0.2f;
            alpha = alpha * alpha * (3f - 2f * alpha) * 0.75f;

            s.Sprite.Modulate = new Color(s.BaseColor.R, s.BaseColor.G, s.BaseColor.B, alpha);
            _streaks[i] = s;
        }
    }

    private void SpawnStreak(float initialAge)
    {
        var sprite = new Sprite2D();
        sprite.Texture = _texture;
        sprite.Material = _mat;

        var scale = _rng.RandfRange(0.5f, 1.0f) * _s;

        sprite.Position = new Vector2(
            _rng.RandfRange(250f, 438f) * _s * (scale / _s),
            _rng.RandfRange(-325f, -125f) * _s
        );

        var vx = _rng.RandfRange(-413f, -288f) * _s;
        var vy = _rng.RandfRange(225f, 275f) * _s;

        var ax = 75f * (scale / _s) * _s;
        var ay = -106f * _s;

        var r = _rng.RandfRange(0.2f, 0.3f);
        var g = _rng.RandfRange(0.65f, 0.8f);
        var baseColor = new Color(r, g, 1.0f, 0f);
        sprite.Modulate = baseColor;

        sprite.Scale = new Vector2(scale * 0.375f, scale * 1.76f);

        sprite.Rotation = Mathf.Atan2(vy, vx) + Mathf.Pi * 0.5f;

        var behind = _rng.Randf() < 0.2f + (scale / _s - 0.5f);
        sprite.ZIndex = behind ? -1 : 1;

        AddChild(sprite);

        _streaks.Add(new StreakData
        {
            Sprite = sprite,
            Age = initialAge,
            Lifetime = _rng.RandfRange(MinLifetime, MaxLifetime),
            VelocityX = vx,
            VelocityY = vy,
            AccelX = ax,
            AccelY = ay,
            BaseColor = new Color(r, g, 1.0f),
            BaseScale = scale,
            RenderBehind = behind
        });
    }

    private struct StreakData
    {
        public Sprite2D Sprite;
        public float Age;
        public float Lifetime;
        public float VelocityX;
        public float VelocityY;
        public float AccelX;
        public float AccelY;
        public Color BaseColor;
        public float BaseScale;
        public bool RenderBehind;
    }
}