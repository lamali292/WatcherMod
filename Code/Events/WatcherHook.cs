using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Watcher.Code.Stances;

namespace Watcher.Code.Events;

public class WatcherHook
{
    public static Task OnStanceChange(PlayerChoiceContext ctx, Player player, WatcherStanceModel oldStance,
        WatcherStanceModel newStance)
    {
        return HookUtils.Dispatch<IOnStanceChange>(player.Creature.CombatState, ctx, m => m.OnStanceChange(ctx, player, oldStance, newStance));
    }
    
    public static decimal ModifyCalmEnergyGain(ICombatState cs, Player player, int baseAmount)
    {
        return HookUtils.Aggregate<IModifyCalmEnergyGain, int>(cs, baseAmount,
            (m, current) => m.ModifyCalmEnergyGain(player, current));
    }

    public static decimal ModifyWrathDamage(ICombatState cs, Player player, decimal baseMultiplier)
    {
        return HookUtils.Aggregate<IModifyWrathDamage, decimal>(cs, baseMultiplier,
            (m, current) => m.ModifyWrathDamage(player, current));
    }
}

