using Godot;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;
using MegaCrit.Sts2.Core.ValueProps;
using Watcher.Code.Vfx;

namespace Watcher.Code.Stances;

public sealed class WrathStance : WatcherStanceModel
{
    
    public override bool ShouldReceiveCombatHooks => true;

    protected override StanceVfxConfig VfxConfig => new(
        AuraScenePath: "res://Watcher/scenes/watcher_mod/vfx/wrath_aura.tscn",
        EnterSfxPath: "res://Watcher/audio/wrath_enter.ogg",
        AmbienceLoopPath: "res://Watcher/audio/wrath_loop.ogg",
        ScreenFlashColor: new Color(1f, 0.15f, 0.1f),
        ScreenShakeStrength: ShakeStrength.Medium
    );

    public override decimal ModifyDamageMultiplicative(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if ((dealer == Owner.Creature || target == Owner.Creature) && !props.HasFlag(ValueProp.Unpowered))
            return 2m;
        return 1m;
    }
}