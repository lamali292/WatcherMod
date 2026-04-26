using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Watcher.Code.Stances;

namespace Watcher.Code.Events;

public class WatcherHook
{
    private static async Task Dispatch<T>(PlayerChoiceContext ctx, Player player, Func<T, Task> invoke)
        where T : class
    {
        var combatState = player.Creature.CombatState;
        if (combatState == null) return;
        foreach (var model in combatState.IterateHookListeners().OfType<T>())
        {
            var abstractModel = (AbstractModel)(object)model;
            ctx.PushModel(abstractModel);
            await invoke(model);
            ctx.PopModel(abstractModel);
        }
    }
    
    private static TResult Aggregate<T, TResult>(CombatState combatState, TResult seed,
        Func<T, TResult, TResult> action)
        where T : class => 
        combatState.IterateHookListeners().OfType<T>()
            .Aggregate(seed, (current, model) => action(model, current));



    public static Task OnStanceChange(PlayerChoiceContext ctx, Player player, WatcherStanceModel oldStance, WatcherStanceModel newStance)
        => Dispatch<IOnStanceChange>(ctx, player, m => m.OnStanceChange(ctx, player, oldStance, newStance));

    public static Task OnScryed(PlayerChoiceContext ctx, Player player, int amount, int discardedAmount)
        => Dispatch<IOnScryed>(ctx, player, m => m.OnScryed(ctx, player, amount, discardedAmount));
    
    public static decimal ModifyCalmEnergyGain(CombatState cs, Player player, int baseAmount) =>
        Aggregate<IModifyCalmEnergyGain, int>(cs, baseAmount,
            (m, current) => m.ModifyCalmEnergyGain(player, current));

    public static decimal ModifyWrathDamage(CombatState cs, Player player, decimal baseMultiplier) =>
        Aggregate<IModifyWrathDamage, decimal>(cs, baseMultiplier,
            (m, current) => m.ModifyWrathDamage(player, current));
}