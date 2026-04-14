using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;
using MegaCrit.Sts2.Core.ValueProps;
using Watcher.Code.Commands;

namespace Watcher.Code.Stances;

public class DivinityStance : StancePower
{
    protected override string AuraScenePath => "res://Watcher/scenes/watcher_mod/vfx/divinity_aura.tscn";
    protected override Color? BodyTint => new Color(1.1f, 0.7f, 1.4f, 1f);
    protected override string EyeTexturePath => "res://Watcher/images/watcher_parts/eye_divinity.png";
    protected override string EnterSfxPath => "res://Watcher/audio/divinity_enter.ogg";
    protected override string AmbienceLoopPath => "res://Watcher/audio/divinity_loop.ogg";
    protected override Color? ScreenFlashColor => new Color(0.8f, 0.3f, 1f, 1f);
    protected override ShakeStrength ScreenShakeStrength => ShakeStrength.Strong;

    public override Task OnEnterStance(Creature creature)
    {
        if (creature.IsPlayer) creature.Player!.PlayerCombatState!.GainEnergy(3);
        return base.OnEnterStance(creature);
    }

    // Deal triple damage
    public override decimal ModifyDamageMultiplicative(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (dealer == Owner && !props.HasFlag(ValueProp.Unpowered))
            return 3m;
        return 1m;
    }

    // Exit Divinity at start of next turn
    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side,
        CombatState combatState)
    {
        if (side != Owner.Side) return;

        await StanceCmd.ExitStance(Owner, null);

        await base.BeforeSideTurnStart(choiceContext, side, combatState);
    }
}
