// src/WatcherMod/Models/Stances/StancePower.cs

using BaseLib.Abstracts;
using BaseLib.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using Watcher.Code.Extensions;

namespace Watcher.Code.Stances;

public abstract class StancePower : CustomPowerModel
{
    // ---------- Aura System ----------
    private Node2D? _vfxInstance;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.None;
    protected override bool IsVisibleInternal => false;

    // Path to a PackedScene for aura; override in your stance classes
    protected virtual string? AuraScenePath => null;

    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    public override string CustomBigIconPath => CustomPackedIconPath;

    // Called when entering a stance
    public virtual async Task OnEnterStance(Creature owner)
    {
        await CreateAura(owner);
    }

    // Called when exiting a stance
    public virtual async Task OnExitStance(Creature owner)
    {
        RemoveAura();
        await Task.CompletedTask;
    }

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

        return Task.CompletedTask;
    }


    private void RemoveAura()
    {
        if (_vfxInstance == null || !GodotObject.IsInstanceValid(_vfxInstance)) return;

        // Safely remove and nullify
        _vfxInstance.QueueFree();
        _vfxInstance = null;
    }
}