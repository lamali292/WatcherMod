using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Commands;
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
    // One controller per stance instance (and so per player): state must not be shared between players.
    private Color? _originalModulate;
    private string? _ambienceLoop;

    // The game keeps a LIST of FMOD instances per loop path and StopLoop pops only one, so every start we
    // make has to be matched by a stop, or the loops pile up and overlap. Counted so they can all be ended.
    private static readonly Dictionary<string, int> ActiveLoops = new();
    private Node2D? _vfxInstance;

    public async Task OnEnter(Creature owner)
    {
        await CreateAura(owner);
        ApplyBodyTint(owner);
        PlayEnterSfx();
        if (LocalContext.IsMe(owner))
        {
            StartAmbience();
            PlayScreenFlash();
            PlayScreenShake();
        }
    }

    public async Task OnExit(Creature owner)
    {
        Cleanup(owner);
        await Task.CompletedTask;
    }

    /// <summary>Synchronous teardown, also used when combat ends without a regular stance exit.</summary>
    public void Cleanup(Creature owner)
    {
        RemoveAura();
        ResetBodyTint(owner);
        StopAmbience();
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
                case WrathGlowSparkSpawner sparks: sparks.StopSpawning(); break;
                case CalmFrostStreakSpawner streaks: streaks.StopSpawning(); break;
                case DivinityEyeSpawner eyes: eyes.StopSpawning(); break;
                case AuraBlobEmitter blob:
                    foreach (var cpu in blob.GetChildren().OfType<CpuParticles2D>())
                        cpu.Emitting = false;
                    var timer = blob.GetTree().CreateTimer(2.5f);
                    timer.Timeout += () =>
                    {
                        if (GodotObject.IsInstanceValid(blob)) blob.QueueFree();
                    };
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
        try
        {
            if (cfg.EnterSfxPath != null)
                SfxCmd.Play(cfg.EnterSfxPath);
        }
        catch (Exception e)
        {
            WatcherMainFile.Logger.Warn(e.ToString());
        }
        
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

        // Local player only; the event fades in/out itself (see fmod/), StopLoop just stops it with its release.
        StopAmbience();
        _ambienceLoop = cfg.AmbienceLoopPath;
        SfxCmd.PlayLoop(_ambienceLoop, false);
        ActiveLoops[_ambienceLoop] = ActiveLoops.GetValueOrDefault(_ambienceLoop) + 1;
    }

    /// <summary>Ends every stance loop still running (combat start/end, or a stance that never got its exit).</summary>
    public static void StopAllAmbience()
    {
        foreach (var (path, count) in ActiveLoops)
            for (var i = 0; i < count; i++)
                SfxCmd.StopLoop(path);
        ActiveLoops.Clear();
    }

    private void StopAmbience()
    {
        if (_ambienceLoop == null) return;

        SfxCmd.StopLoop(_ambienceLoop);
        if (ActiveLoops.TryGetValue(_ambienceLoop, out var n))
        {
            if (n <= 1) ActiveLoops.Remove(_ambienceLoop);
            else ActiveLoops[_ambienceLoop] = n - 1;
        }

        _ambienceLoop = null;
    }
}