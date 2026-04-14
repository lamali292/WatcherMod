using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Watcher.Code.Cards.CardModels;
using Watcher.Code.Stances;

namespace Watcher.Code.Commands;

public static class StanceCmd
{
    /// <summary>
    ///     Event triggered whenever a creature changes stance.
    ///     Powers can subscribe to this for stance-dependent effects.
    /// </summary>
    public static event Func<Creature, PlayerChoiceContext?, Task>? StanceChanged;

    /// <summary>
    ///     Change a creature's stance. Handles exit/enter hooks and notifications.
    /// </summary>
    private static async Task Execute(Creature creature, StancePower? newStance, PlayerChoiceContext? context)
    {
        var currentStance = GetCurrentStance(creature);

        // Early exit if no actual change is happening
        if (!ShouldChangeStance(currentStance, newStance))
            return;

        // Exit current stance
        await ExitCurrentStance(currentStance, creature);

        // Enter new stance
        await EnterNewStance(newStance, creature);

        // Notify all listeners about the stance change (pass context!)
        await NotifyPowers(creature, context);
        await NotifyCards(creature);
    }

    public static async Task EnterWrath(Creature creature, PlayerChoiceContext? context)
    {
        await Execute(creature, ModelDb.Power<WrathStance>(), context);
    }

    public static async Task EnterCalm(Creature creature, PlayerChoiceContext? context)
    {
        await Execute(creature, ModelDb.Power<CalmStance>(), context);
    }

    public static async Task EnterDivinity(Creature creature, PlayerChoiceContext? context)
    {
        await Execute(creature, ModelDb.Power<DivinityStance>(), context);
    }

    public static async Task ExitStance(Creature creature, PlayerChoiceContext? context)
    {
        await Execute(creature, null, context);
    }


    private static async Task ExitCurrentStance(StancePower? currentStance, Creature creature)
    {
        if (currentStance == null)
            return;

        await currentStance.OnExitStance(creature);
        currentStance.RemoveInternal();
    }

    private static StancePower? GetCurrentStance(Creature creature)
    {
        return creature.Powers.OfType<StancePower>().FirstOrDefault();
    }

    private static bool ShouldChangeStance(StancePower? currentStance, StancePower? newStance)
    {
        // Both null - no change
        if (currentStance == null && newStance == null)
            return false;

        // Switching to the same stance - no change
        return currentStance == null || newStance == null ||
               currentStance.GetType() != newStance.GetType();
    }


    private static async Task EnterNewStance(StancePower? newStance, Creature creature)
    {
        if (newStance == null)
            return;

        var mutableStance = newStance.ToMutable();
        mutableStance.ApplyInternal(creature, 1);
        await ((StancePower)mutableStance).OnEnterStance(creature);
    }

    private static async Task NotifyPowers(Creature creature, PlayerChoiceContext? context)
    {
        if (StanceChanged != null) await StanceChanged.Invoke(creature, context);
    }


    private static async Task NotifyCards(Creature creature)
    {
        var player = creature.Player;

        // Iterate through ALL piles (Hand, Draw, Discard, Exhaust, etc.)
        var allPiles = player?.PlayerCombatState?.AllPiles;
        if (allPiles != null)
            foreach (var pile in allPiles)
            {
                // Make a copy to avoid modification during iteration
                var watcherCards = pile.Cards.OfType<WatcherCardModel>().ToList();

                foreach (var card in watcherCards) await card.OnStanceChanged(creature);
            }
    }
}