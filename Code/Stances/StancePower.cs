using BaseLib.Abstracts;
using BaseLib.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;
using Watcher.Code.Extensions;

namespace Watcher.Code.Stances;

public abstract class StancePower : CustomPowerModel
{
    // ---------- Aura System ----------
    private Node2D? _vfxInstance;

    // ---------- Eye Decoration (shared across stances) ----------
    private static Node2D? _eyeBoneNode;
    private static Sprite2D? _irisSprite;
    private static Sprite2D? _lidUpperSprite;
    private static Sprite2D? _lidLowerSprite;

    // ---------- Body Tint ----------
    private static Color? _originalModulate;

    // ---------- Looping Ambience ----------
    private static AudioStreamPlayer? _ambiencePlayer;
    private const float AmbienceFadeTime = 0.6f;
    private const float AmbienceVolume = -6f; // dB

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.None;
    protected override bool IsVisibleInternal => false;

    // Path to a PackedScene for aura; override in your stance classes
    protected virtual string? AuraScenePath => null;

    // Override for stance-specific body color tint (null = no tint)
    protected virtual Color? BodyTint => null;

    // Override for stance-specific eye iris texture path (null = closed eye)
    protected virtual string? EyeTexturePath => null;

    // Override for stance enter SFX path
    protected virtual string? EnterSfxPath => null;

    // Override for screen flash color on stance enter (null = no flash)
    protected virtual Color? ScreenFlashColor => null;

    // Override for screen shake on stance enter
    protected virtual ShakeStrength ScreenShakeStrength => ShakeStrength.None;

    // Override for looping ambience path (null = no ambience)
    protected virtual string? AmbienceLoopPath => null;

    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    public override string CustomBigIconPath => CustomPackedIconPath;

    public virtual async Task OnEnterStance(Creature owner)
    {
        await CreateAura(owner);
        ApplyBodyTint(owner);
        EnsureEyeSetup(owner);
        UpdateEye(true);
        PlayEnterSfx();
        StartAmbience();
        if (LocalContext.IsMe(owner))
        {
            PlayScreenFlash();
            PlayScreenShake();
        }
    }

    public virtual async Task OnExitStance(Creature owner)
    {
        RemoveAura();
        ResetBodyTint(owner);
        UpdateEye(false);
        StopAmbience();
        await Task.CompletedTask;
    }

    // ──────────────────────────────────────────────
    //  Aura Scene
    // ──────────────────────────────────────────────

    private Task CreateAura(Creature owner)
    {
        if (AuraScenePath == null) return Task.CompletedTask;

        var creatureNode = NCombatRoom.Instance?.GetCreatureNode(owner);
        var visuals = creatureNode?.Visuals;
        if (visuals == null) return Task.CompletedTask;

        var container = visuals.GetNodeOrNull<Node2D>("StanceVfxContainer");
        if (container == null)
        {
            container = new Node2D { Name = "StanceVfxContainer", Position = Vector2.Zero };
            visuals.AddChild(container);
        }

        if (_vfxInstance != null && GodotObject.IsInstanceValid(_vfxInstance))
            _vfxInstance.QueueFree();

        var scene = PreloadManager.Cache.GetScene(AuraScenePath);
        _vfxInstance = scene.Instantiate<Node2D>();
        _vfxInstance.Position = Vector2.Zero;
        _vfxInstance.Scale = Vector2.One;
        container.AddChild(_vfxInstance);

        // Move burst nodes behind the character by reparenting to visuals
        // as the first child (drawn before the body sprite)
        var bursts = new System.Collections.Generic.List<Node>();
        foreach (var child in _vfxInstance.GetChildren())
        {
            if (child.Name.ToString().Contains("Burst"))
                bursts.Add(child);
        }
        foreach (var burst in bursts)
        {
            var pos = ((Node2D)burst).GlobalPosition;
            burst.Reparent(visuals);
            ((Node2D)burst).GlobalPosition = pos;
            visuals.MoveChild(burst, 0);
        }

        return Task.CompletedTask;
    }

    private void RemoveAura()
    {
        if (_vfxInstance == null || !GodotObject.IsInstanceValid(_vfxInstance)) return;

        // Stop spawners — they self-destruct when their particles finish
        foreach (var child in _vfxInstance.GetChildren())
        {
            if (child is WrathGlowSparkSpawner sparks) sparks.StopSpawning();
            else if (child is CalmFrostStreakSpawner streaks) streaks.StopSpawning();
            else if (child is DivinityEyeSpawner eyes) eyes.StopSpawning();
            else if (child is AuraBlobEmitter)
            {
                foreach (var sub in child.GetChildren())
                    if (sub is CpuParticles2D cpu) cpu.Emitting = false;
                var timer = child.GetTree().CreateTimer(2.5f);
                var c = child;
                timer.Timeout += () => { if (GodotObject.IsInstanceValid(c)) c.QueueFree(); };
            }
        }

        _vfxInstance = null;
    }

    // ──────────────────────────────────────────────
    //  Body Tint
    // ──────────────────────────────────────────────

    private void ApplyBodyTint(Creature owner)
    {
        if (BodyTint == null) return;

        var creatureNode = NCombatRoom.Instance?.GetCreatureNode(owner);
        if (creatureNode == null) return;

        var body = (Node2D)creatureNode.Body;
        _originalModulate ??= body.Modulate;
        body.Modulate = BodyTint.Value;
    }

    private void ResetBodyTint(Creature owner)
    {
        if (_originalModulate == null) return;

        var creatureNode = NCombatRoom.Instance?.GetCreatureNode(owner);
        if (creatureNode == null) return;

        var body = (Node2D)creatureNode.Body;
        body.Modulate = _originalModulate.Value;
        _originalModulate = null;
    }

    // ──────────────────────────────────────────────
    //  Eye Decoration (SpineBoneNode on eye_anchor)
    // ──────────────────────────────────────────────

    /// <summary>
    ///     Creates the eye bone node with iris and lid sprites if not already present.
    ///     Can be called from outside (e.g. StanceCmd) to ensure lids are visible
    ///     even before any stance is entered.
    /// </summary>
    public static void EnsureEyeSetup(Creature owner)
    {
        var creatureNode = NCombatRoom.Instance?.GetCreatureNode(owner);
        var body = creatureNode?.Body;
        if (body == null) return;
        EnsureEyeSetup(body);
    }

    public static void EnsureEyeSetup(Node2D body)
    {
        if (_eyeBoneNode != null && GodotObject.IsInstanceValid(_eyeBoneNode)) return;

        // SpineBoneNode is a GDExtension type — instantiate via ClassDB
        var variant = ClassDB.Instantiate(new StringName("SpineBoneNode"));
        _eyeBoneNode = variant.As<Node2D>();
        if (_eyeBoneNode == null)
        {
            GD.PrintErr("[StancePower] Failed to create SpineBoneNode for eye decoration");
            return;
        }
        _eyeBoneNode.Set("bone_name", "eye_anchor");
        _eyeBoneNode.Name = "EyeDecoNode";

        // Eye sprites need rotation to align with the staff angle
        // and PremultAlpha blend to avoid dark seams (atlas is premultiplied)
        const float eyeRotation = -0.500909f;
        var eyeMat = new CanvasItemMaterial { BlendMode = CanvasItemMaterial.BlendModeEnum.PremultAlpha };

        var baseSprite = new Sprite2D
        {
            Name = "Base",
            Texture = GD.Load<Texture2D>("res://Watcher/images/watcher_parts/eye_base.png"),
            Visible = true,
            Rotation = eyeRotation,
            Material = eyeMat
        };
        _eyeBoneNode.AddChild(baseSprite);

        _irisSprite = new Sprite2D
        {
            Name = "Iris", Visible = false, Rotation = eyeRotation, Material = eyeMat
        };
        _eyeBoneNode.AddChild(_irisSprite);

        _lidUpperSprite = new Sprite2D
        {
            Name = "LidUpper",
            Texture = GD.Load<Texture2D>("res://Watcher/images/watcher_parts/eye_lid_upper.png"),
            Visible = true,
            Rotation = eyeRotation,
            Material = eyeMat
        };
        _eyeBoneNode.AddChild(_lidUpperSprite);

        _lidLowerSprite = new Sprite2D
        {
            Name = "LidLower",
            Texture = GD.Load<Texture2D>("res://Watcher/images/watcher_parts/eye_lid_lower.png"),
            Visible = true,
            Rotation = eyeRotation,
            Material = eyeMat
        };
        _eyeBoneNode.AddChild(_lidLowerSprite);

        body.AddChild(_eyeBoneNode);
    }

    private void UpdateEye(bool opening)
    {
        if (_irisSprite == null || !GodotObject.IsInstanceValid(_irisSprite)) return;

        if (opening && EyeTexturePath != null)
        {
            _irisSprite.Texture = GD.Load<Texture2D>(EyeTexturePath);
            _irisSprite.Visible = true;
            _irisSprite.Scale = new Vector2(0.8f, 0.8f);
            if (_lidUpperSprite != null) _lidUpperSprite.Visible = false;
            if (_lidLowerSprite != null) _lidLowerSprite.Visible = false;
        }
        else
        {
            _irisSprite.Visible = false;
            _irisSprite.Scale = Vector2.One;
            if (_lidUpperSprite != null) _lidUpperSprite.Visible = true;
            if (_lidLowerSprite != null) _lidLowerSprite.Visible = true;
        }
    }

    // ──────────────────────────────────────────────
    //  SFX
    // ──────────────────────────────────────────────

    private void PlayEnterSfx()
    {
        if (EnterSfxPath == null) return;
        StanceVfx.PlayStanceSfx(EnterSfxPath);
    }

    // ──────────────────────────────────────────────
    //  Screen Effects
    // ──────────────────────────────────────────────

    private void PlayScreenFlash()
    {
        if (ScreenFlashColor == null) return;
        ScreenFlashEffect.Play(ScreenFlashColor.Value);
    }

    private void PlayScreenShake()
    {
        if (ScreenShakeStrength == ShakeStrength.None) return;
        NGame.Instance?.ScreenShake(ScreenShakeStrength, ShakeDuration.Short);
    }

    // ──────────────────────────────────────────────
    //  Looping Ambience
    // ──────────────────────────────────────────────

    private void StartAmbience()
    {
        if (AmbienceLoopPath == null) return;

        // Stop any existing ambience first (no fade, immediate swap)
        if (_ambiencePlayer != null && GodotObject.IsInstanceValid(_ambiencePlayer))
            _ambiencePlayer.QueueFree();

        var stream = GD.Load<AudioStream>(AmbienceLoopPath);
        if (stream == null) return;

        _ambiencePlayer = new AudioStreamPlayer();
        _ambiencePlayer.Stream = stream;
        _ambiencePlayer.Bus = "SFX";
        _ambiencePlayer.VolumeDb = -80f;

        var combatRoom = NCombatRoom.Instance;
        if (combatRoom == null) return;

        combatRoom.AddChild(_ambiencePlayer);
        _ambiencePlayer.Play();

        // Fade in
        var tween = _ambiencePlayer.CreateTween();
        tween.TweenProperty(_ambiencePlayer, "volume_db", AmbienceVolume, AmbienceFadeTime);
    }

    private static void StopAmbience()
    {
        if (_ambiencePlayer == null || !GodotObject.IsInstanceValid(_ambiencePlayer)) return;

        var player = _ambiencePlayer;
        _ambiencePlayer = null;

        // Fade out then free
        var tween = player.CreateTween();
        tween.TweenProperty(player, "volume_db", -80f, AmbienceFadeTime);
        tween.TweenCallback(Callable.From(() =>
        {
            if (GodotObject.IsInstanceValid(player)) player.QueueFree();
        }));
    }
}
