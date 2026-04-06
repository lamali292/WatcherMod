using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;
using MegaCrit.Sts2.Core.ValueProps;
using Watcher.Code.Commands;
using Watcher.Code.Vfx;

namespace Watcher.Code.Stances;

public class DivinityStance : WatcherStanceModel
{
    public override bool ShouldReceiveCombatHooks => true;
    protected override StanceVfxConfig VfxConfig => new(
        AuraScenePath: "res://Watcher/scenes/watcher_mod/vfx/divinity_aura.tscn",
        BodyTint: new Color(1.1f, 0.7f, 1.4f),
        EnterSfxPath: "res://Watcher/audio/divinity_enter.ogg",
        AmbienceLoopPath: "res://Watcher/audio/divinity_loop.ogg",
        ScreenFlashColor: new Color(0.8f, 0.3f, 1f),
        ScreenShakeStrength: ShakeStrength.Strong
    );

    public override Task OnEnterStance(PlayerChoiceContext ctx, Player player, CardModel? source)
    {
        player.PlayerCombatState!.GainEnergy(3);
        return base.OnEnterStance(ctx,player,source);
    }

    public override decimal ModifyDamageMultiplicative(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (dealer == Owner.Creature && !props.HasFlag(ValueProp.Unpowered))
            return 3m;
        return 1m;
    }

    public override async Task BeforeSideTurnStart(PlayerChoiceContext ctx, CombatSide side,
        CombatState combatState)
    {
        if (side != Owner.Creature.Side) return;
        await StanceCmd.ExitStance(ctx, Owner, null);
    }
}