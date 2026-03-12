using MegaCrit.Sts2.Core.Entities.Creatures;
using WatcherMod.Relics;

namespace WatcherMod.Models.Stances;

public class CalmStance : StancePower
{
    protected override string AuraScenePath => "res://scenes/watcher_mod/vfx/calm_aura.tscn";

    public override Task OnExitStance(Creature creature)
    {
        var amount = 2;
        if (creature.Player?.GetRelic<VioletLotus>() != null) amount += 1;
        if (creature.IsPlayer) creature.Player!.PlayerCombatState!.GainEnergy(amount);
        return base.OnExitStance(creature);
    }
}