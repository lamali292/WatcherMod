using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;
using Watcher.Code.Stances.Vfx;
using Watcher.Code.Vfx;

namespace Watcher.Code.Stances;

public class StanceVfxController(StanceVfxConfig cfg)
{
    private const float AmbienceFadeTime = 0.6f;
    private const float AmbienceVolume = -6f;

    private static Color? _originalModulate;
    private static AudioStreamPlayer? _ambiencePlayer;
    private Node2D? _vfxInstance;

    public async Task OnEnter(Creature owner)
    {
        await CreateAura(owner);
        ApplyBodyTint(owner);
        PlayEnterSfx();
        StartAmbience();
        if (LocalContext.IsMe(owner))
        {
            PlayScreenFlash();
            PlayScreenShake();
        }
    }

    public async Task OnExit(Creature owner)
    {
        RemoveAura();
        ResetBodyTint(owner);
        StopAmbience();
        await Task.CompletedTask;
    }

    // ── Aura ──────────────────────────────────────

    private Task CreateAura(Creature owner)
    {
        if (cfg.AuraScenePath == null) return Task.CompletedTask;

        var visuals = NCombatRoom.Instance?.GetCreatureNode(owner)?.Visuals;
        if (visuals == null) return Task.CompletedTask;

        var container = visuals.GetNodeOrNull<Node2D>("StanceVfxContainer")
                        ?? CreateContainer(visuals);

        if (_vfxInstance != null && GodotObject.IsInstanceValid(_vfxInstance))
            _vfxInstance.QueueFree();

        _vfxInstance = PreloadManager.Cache.GetScene(cfg.AuraScenePath).Instantiate<Node2D>();
        _vfxInstance.Position = Vector2.Zero;
        _vfxInstance.Scale = Vector2.One;
        container.AddChild(_vfxInstance);

        foreach (var burst in _vfxInstance.GetChildren()
                     .Where(c => c.Name.ToString().Contains("Burst"))
                     .Cast<Node2D>())
        {
            var pos = burst.GlobalPosition;
            burst.Reparent(visuals);
            burst.GlobalPosition = pos;
            visuals.MoveChild(burst, 0);
        }

        return Task.CompletedTask;
    }

    private static Node2D CreateContainer(Node visuals)
    {
        var c = new Node2D { Name = "StanceVfxContainer", Position = Vector2.Zero };
        visuals.AddChild(c);
        return c;
    }

    private void RemoveAura()
    {
        if (_vfxInstance == null || !GodotObject.IsInstanceValid(_vfxInstance)) return;

        foreach (var child in _vfxInstance.GetChildren())
            switch (child)
            {
                case WrathGlowSparkSpawner sparks:   sparks.StopSpawning();  break;
                case CalmFrostStreakSpawner streaks:  streaks.StopSpawning(); break;
                case DivinityEyeSpawner eyes:         eyes.StopSpawning();   break;
                case AuraBlobEmitter blob:
                    foreach (var cpu in blob.GetChildren().OfType<CpuParticles2D>())
                        cpu.Emitting = false;
                    var timer = blob.GetTree().CreateTimer(2.5f);
                    timer.Timeout += () => { if (GodotObject.IsInstanceValid(blob)) blob.QueueFree(); };
                    break;
            }

        _vfxInstance = null;
    }

    // ── Body Tint ─────────────────────────────────

    private void ApplyBodyTint(Creature owner)
    {
        if (cfg.BodyTint == null) return;
        var body = NCombatRoom.Instance?.GetCreatureNode(owner)?.Body;
        if (body == null) return;
        _originalModulate ??= body.Modulate;
        body.Modulate = cfg.BodyTint.Value;
    }

    private void ResetBodyTint(Creature owner)
    {
        if (_originalModulate == null) return;
        var body = NCombatRoom.Instance?.GetCreatureNode(owner)?.Body;
        if (body == null) return;
        body.Modulate = _originalModulate.Value;
        _originalModulate = null;
    }

    // ── SFX ───────────────────────────────────────

    private void PlayEnterSfx()
    {
        if (cfg.EnterSfxPath != null)
            StanceVfx.PlayStanceSfx(cfg.EnterSfxPath);
    }

    private void PlayScreenFlash()
    {
        if (cfg.ScreenFlashColor != null)
            ScreenFlashEffect.Play(cfg.ScreenFlashColor.Value);
    }

    private void PlayScreenShake()
    {
        if (cfg.ScreenShakeStrength != ShakeStrength.None)
            NGame.Instance?.ScreenShake(cfg.ScreenShakeStrength, ShakeDuration.Short);
    }

    // ── Ambience ──────────────────────────────────

    private void StartAmbience()
    {
        if (cfg.AmbienceLoopPath == null) return;

        if (_ambiencePlayer != null && GodotObject.IsInstanceValid(_ambiencePlayer))
            _ambiencePlayer.QueueFree();

        var combatRoom = NCombatRoom.Instance;
        if (combatRoom == null) return;

        _ambiencePlayer = new AudioStreamPlayer
        {
            Stream = PreloadManager.Cache.GetAsset<AudioStream>(cfg.AmbienceLoopPath),
            Bus = "SFX",
            VolumeDb = -80f
        };

        combatRoom.AddChild(_ambiencePlayer);
        _ambiencePlayer.Play();

        _ambiencePlayer.CreateTween()
            .TweenProperty(_ambiencePlayer, "volume_db", AmbienceVolume, AmbienceFadeTime);
    }

    private static void StopAmbience()
    {
        if (_ambiencePlayer == null || !GodotObject.IsInstanceValid(_ambiencePlayer)) return;

        var player = _ambiencePlayer;
        _ambiencePlayer = null;

        var tween = player.CreateTween();
        tween.TweenProperty(player, "volume_db", -80f, AmbienceFadeTime);
        tween.TweenCallback(Callable.From(() =>
        {
            if (GodotObject.IsInstanceValid(player)) player.QueueFree();
        }));
    }
}