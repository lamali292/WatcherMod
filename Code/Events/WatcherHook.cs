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

    private static TResult Aggregate<T, TResult>(ICombatState combatState, TResult seed,
        Func<T, TResult, TResult> action)
        where T : class
    {
        return combatState.IterateHookListeners().OfType<T>()
            .Aggregate(seed, (current, model) => action(model, current));
    }

    
      /// <summary>
    ///     Passes a value through all hook listeners of type <typeparamref name="THook" />,
    ///     tracking which listeners actually modified the value.
    /// </summary>
    /// <typeparam name="THook">The hook interface to filter listeners by.</typeparam>
    /// <typeparam name="TValue">The type of the value being modified. Must implement <see cref="IEquatable{T}" />.</typeparam>
    /// <param name="combatState">The current combat state to iterate listeners from.</param>
    /// <param name="originalAmount">The initial value before any modifications.</param>
    /// <param name="amountModifier">A function that takes a listener and the current value and returns the modified value.</param>
    /// <param name="modifiers">Outputs the set of listeners that actually changed the value.</param>
    /// <returns>The final modified value after all listeners have been processed.</returns>
    public static TValue Modify<THook, TValue>(
        ICombatState? combatState,
        TValue originalAmount,
        Func<THook, TValue, TValue> amountModifier,
        out IEnumerable<THook> modifiers)
        where THook : class
        where TValue : IEquatable<TValue>
    {
        if (combatState == null)
        {
            modifiers = [];
            return originalAmount;
        }
        var amount = originalAmount;
        var abstractModelList = new List<THook>();
        foreach (var model in combatState.IterateHookListeners().OfType<THook>())
        {
            var previous = amount;
            amount = amountModifier.Invoke(model, amount);
            if (!previous.Equals(amount))
                abstractModelList.Add(model);
        }

        modifiers = abstractModelList;
        return amount;
    }

    /// <summary>
    ///     Invokes a follow-up action on all listeners that previously modified a value via
    ///     <see cref="Modify{THook,TValue}" />,
    ///     iterating in hook listener order and invoking <see cref="AbstractModel.InvokeExecutionFinished" /> for each.
    /// </summary>
    /// <typeparam name="THook">The hook interface to filter listeners by.</typeparam>
    /// <param name="cs">The current combat state to iterate listeners from.</param>
    /// <param name="modifiers">
    ///     The set of listeners that modified the value, as returned by
    ///     <see cref="Modify{THook,TValue}" />.
    /// </param>
    /// <param name="action">The async action to invoke on each modifier.</param>
    public static async Task AfterModifying<THook>(ICombatState cs, IEnumerable<THook> modifiers,
        Func<THook, Task> action)
        where THook : class
    {
        var modifierSet = new HashSet<THook>(modifiers);
        foreach (var iterateHookListener in cs.IterateHookListeners().OfType<THook>())
        {
            if (!modifierSet.Contains(iterateHookListener)) continue;
            await action(iterateHookListener);
            if (iterateHookListener is AbstractModel model)
                model.InvokeExecutionFinished();
        }
    }
    
    
    public static Task OnStanceChange(PlayerChoiceContext ctx, Player player, WatcherStanceModel oldStance,
        WatcherStanceModel newStance)
    {
        return Dispatch<IOnStanceChange>(ctx, player, m => m.OnStanceChange(ctx, player, oldStance, newStance));
    }

    /// <summary>
    ///     Dispatches <see cref="IAfterScryed.AfterScryed" /> to all subscribed models after a scry
    ///     has fully resolved (viewed cards chosen, discards moved to the discard pile).
    ///     <para>
    ///         Only fires when a scry actually happened: the modified scry amount was greater than
    ///         zero <b>and</b> the draw pile contained at least one card. Scries that resolve to
    ///         nothing (amount reduced to 0 by modifiers, or an empty draw pile) never reach this hook.
    ///     </para>
    /// </summary>
    /// <param name="ctx">Player choice context of the ongoing player decision.</param>
    /// <param name="player">The player who scryed.</param>
    /// <param name="scryAmount">
    ///     The scry amount after <see cref="WatcherHook.ModifyScryAmount" /> modifiers, as requested —
    ///     <b>not</b> clamped to the draw pile size. May exceed the number of cards actually viewed
    ///     (e.g. Scry 5 with 2 cards in the draw pile passes 5).
    /// </param>
    /// <param name="discardedAmount">
    ///     Number of cards the player chose to discard; always equals the count of
    ///     <paramref name="discarded" />. May be 0 when the player kept everything.
    /// </param>
    /// <param name="discarded">
    ///     The cards discarded by this scry, already added to the discard pile by the time this hook runs. Empty when the
    ///     player kept everything.
    /// </param>
    public static Task AfterScryed(PlayerChoiceContext ctx, Player player, int scryAmount, int discardedAmount, IEnumerable<CardModel> discarded)
    {
        return Dispatch<IAfterScryed>(ctx, player, m => m.AfterScryed(ctx, player, scryAmount, discardedAmount, discarded));
    }

    public static decimal ModifyCalmEnergyGain(ICombatState cs, Player player, int baseAmount)
    {
        return Aggregate<IModifyCalmEnergyGain, int>(cs, baseAmount,
            (m, current) => m.ModifyCalmEnergyGain(player, current));
    }

    public static decimal ModifyWrathDamage(ICombatState cs, Player player, decimal baseMultiplier)
    {
        return Aggregate<IModifyWrathDamage, decimal>(cs, baseMultiplier,
            (m, current) => m.ModifyWrathDamage(player, current));
    }

    public static int ModifyScryAmount(Player player, int amount, out IEnumerable<IModifyScryAmount> modifiers)
    {
        return Modify(player.Creature.CombatState, amount, (m, a) => m.ModifyScryAmount(player, a), out modifiers);
    }
    
    public static Task AfterModifyingScryAmount(PlayerChoiceContext ctx, Player player, IEnumerable<IModifyScryAmount> modifiers, int originalAmount, int modifiedAmount)
    {
        return AfterModifying(player.Creature.CombatState!, modifiers, a=> a.AfterModifyingScryAmount(ctx, player, originalAmount, modifiedAmount));
    }
}

