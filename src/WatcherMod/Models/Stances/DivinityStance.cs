using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace WatcherMod.Models.Stances;

public class DivinityStance : StancePower
{
    protected override string AuraScenePath => "res://scenes/watcher_mod/vfx/divinity_aura.tscn";

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
        if (dealer == Owner)
            return 3m;
        return 1m;
    }

    // Exit Divinity at start of next turn
    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side,
        CombatState combatState)
    {
        if (side != Owner.Side) return;

        await ChangeStanceCmd.Execute(Owner, null, choiceContext);

        await base.BeforeSideTurnStart(choiceContext, side, combatState);
    }
}