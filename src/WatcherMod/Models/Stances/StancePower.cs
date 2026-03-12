// src/WatcherMod/Models/Stances/StancePower.cs

using Godot;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace WatcherMod.Models.Stances;

public abstract class StancePower : PowerModel
{
    // ---------- Aura System ----------
    private Node2D? _vfxInstance;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.None;
    protected override bool IsVisibleInternal => false;

    // Path to a PackedScene for aura; override in your stance classes
    protected virtual string AuraScenePath => null;

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
        var creatureNode = NCombatRoom.Instance?.GetCreatureNode(owner); // use passed owner

        var visuals = creatureNode?.Visuals;
        if (visuals == null) return Task.CompletedTask;

        // Get or create container
        var container = visuals.GetNodeOrNull<Node2D>("StanceVfxContainer");
        if (container == null)
        {
            container = new Node2D { Name = "StanceVfxContainer", Position = Vector2.Zero };
            visuals.AddChild(container);
        }

        // Remove any previous aura to prevent duplicates
        if (_vfxInstance != null && GodotObject.IsInstanceValid(_vfxInstance)) _vfxInstance.QueueFree();

        var scene = GD.Load<PackedScene>(AuraScenePath);
        if (scene == null) return Task.CompletedTask;

        _vfxInstance = scene.Instantiate<Node2D>();
        _vfxInstance.Position = Vector2.Zero;

        container.AddChild(_vfxInstance);
        _vfxInstance.Scale = Vector2.One;

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