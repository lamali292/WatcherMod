using Godot;
using MegaCrit.Sts2.Core.Assets;

namespace Watcher.Code.Stances.Vfx;

[GlobalClass]
public partial class DivinityEyeSpawner : Node2D
{
    private const float SpawnInterval = 0.2f;

    private readonly List<EyeData> _eyes = new();
    private AtlasTexture[] _frames = null!;
    private CanvasItemMaterial _mat = null!;
    private RandomNumberGenerator _rng = null!;
    private float _s;
    private float _spawnTimer;
    private bool _stopping;

    public void StopSpawning()
    {
        _stopping = true;
    }

    public override void _Ready()
    {
        _s = StanceVfx.VfxScale;
        Position *= _s;

        _rng = new RandomNumberGenerator();
        _rng.Randomize();
        _mat = new CanvasItemMaterial { BlendMode = CanvasItemMaterial.BlendModeEnum.Add };

        var strip = PreloadManager.Cache.GetAsset<Texture2D>("res://Watcher/images/vfx/eye_anim.png");
        _frames = new AtlasTexture[7];
        for (var i = 0; i < 7; i++)
        {
            _frames[i] = new AtlasTexture();
            _frames[i].Atlas = strip;
            _frames[i].Region = new Rect2(i * 64, 0, 64, 64);
        }

        for (var i = 0; i < 3; i++)
        {
            var preAge = _rng.RandfRange(0f, 2.0f);
            SpawnEye(preAge);
        }
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
                SpawnEye(0f);
            }
        }
        else if (_eyes.Count == 0)
        {
            QueueFree();
            return;
        }

        for (var i = _eyes.Count - 1; i >= 0; i--)
        {
            var e = _eyes[i];
            e.Age += dt;

            if (e.Age >= e.Lifetime)
            {
                e.Sprite.QueueFree();
                _eyes.RemoveAt(i);
                continue;
            }

            var progress = e.Age / e.Lifetime;

            var frame = GetEyeFrame(progress);
            if (frame >= 0 && frame < _frames.Length)
                e.Sprite.Texture = _frames[frame];

            var bob = GetVerticalBob(progress, e.Scale);
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
        return progress switch
        {
            < 0.15f => 0,
            < 0.20f => 1,
            < 0.25f => 2,
            < 0.30f => 3,
            < 0.35f => 4,
            < 0.40f => 5,
            < 0.55f => 6,
            < 0.62f => 5,
            < 0.70f => 4,
            < 0.75f => 3,
            < 0.80f => 2,
            < 0.85f => 1,
            _ => 0
        };
    }

    private float GetVerticalBob(float progress, float scale)
    {
        var frame = GetEyeFrame(progress);
        var bobAmount = frame switch
        {
            0 => 12f,
            1 => 8f,
            2 => 4f,
            3 => 3f,
            _ => 0f
        };
        return bobAmount * scale * _s;
    }

    private void SpawnEye(float initialAge)
    {
        var sprite = new Sprite2D();
        sprite.Texture = _frames[0];
        sprite.Material = _mat;

        var scale = _rng.RandfRange(1.25f, 1.88f) * _s;
        sprite.Scale = new Vector2(scale, scale);

        var px = _rng.RandfRange(-150f, 150f) * _s;
        var py = _rng.RandfRange(-175f, 75f) * _s;
        sprite.Position = new Vector2(px, py);

        var rot = _rng.RandfRange(6f, 12f);
        if (px > 0) rot = -rot;
        sprite.Rotation = Mathf.DegToRad(rot);

        var r = _rng.RandfRange(0.8f, 1.0f);
        var g = _rng.RandfRange(0.5f, 0.7f);
        var b = _rng.RandfRange(0.8f, 1.0f);
        var baseColor = new Color(r, g, b, 0f);
        sprite.Modulate = baseColor;

        sprite.ZIndex = _rng.Randf() < 0.5f ? -1 : 1;

        AddChild(sprite);

        var lifetime = scale / _s + 0.8f;

        _eyes.Add(new EyeData
        {
            Sprite = sprite,
            Age = initialAge,
            Lifetime = lifetime,
            Scale = scale,
            BaseColor = new Color(r, g, b),
            BobSpeed = 1f,
            StartY = py
        });
    }

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
}