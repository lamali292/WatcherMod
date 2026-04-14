using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Watcher.Code.Stances;

namespace Watcher.Code.Events;

public class WatcherHook
{
    public static async Task OnStanceChange(PlayerChoiceContext ctx, Player player, WatcherStanceModel oldStance,
        WatcherStanceModel newStance)
    {
        var combatState = player.Creature.CombatState;
        if (combatState == null) return;
        foreach (var model in combatState.IterateHookListeners().OfType<IOnStanceChange>())
        {
            var abstractModel = (AbstractModel)model;
            ctx.PushModel(abstractModel);
            await model.OnStanceChange(ctx, player, oldStance, newStance);
            ctx.PopModel(abstractModel);
        }
    }
    
    public static async Task OnScryed(PlayerChoiceContext ctx, Player player, int amount)
    {
        var combatState = player.Creature.CombatState;
        if (combatState == null) return;
        foreach (var model in combatState.IterateHookListeners().OfType<IOnScryed>())
        {
            var abstractModel = (AbstractModel)model;
            ctx.PushModel(abstractModel);
            await model.OnScryed(ctx, player, amount);
            ctx.PopModel(abstractModel);
        }
    }
}