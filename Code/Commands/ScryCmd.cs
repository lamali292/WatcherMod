using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using Watcher.Code.Events;
using Watcher.Code.Extensions;

namespace Watcher.Code.Commands;

public readonly record struct ScryResult
{
    private readonly IReadOnlyList<CardModel>? _discarded;

    public ScryResult(IReadOnlyList<CardModel> discarded) => _discarded = discarded;

    public IReadOnlyList<CardModel> Discarded => _discarded ?? [];

    public static ScryResult Empty => default;
}

public static class ScryCmd
{
    public static Task<ScryResult> Execute(PlayerChoiceContext choiceContext, CardModel card)
    {
        return Execute(choiceContext, card.Owner, card.DynamicVars.Scry().IntValue);
    }
    
    public static async Task<ScryResult> Execute(PlayerChoiceContext choiceContext, Player player, int amount)
    {
        var modifiedAmount = WatcherHook.ModifyScryAmount(player, amount, out var modifiers);
        await WatcherHook.AfterModifyingScryAmount(choiceContext, player, modifiers, amount, modifiedAmount);

        if (modifiedAmount <= 0) return default;
        
        var drawPile = PileType.Draw.GetPile(player);
        var discardPile = PileType.Discard.GetPile(player);
        var combatState = player.Creature.CombatState;
        if (combatState == null) return default;
        var cardsToScry = drawPile.Cards.Take(modifiedAmount).ToList();
        if (cardsToScry.Count == 0) return default;
        var prefs = new CardSelectorPrefs(
            CardSelectorPrefs.DiscardSelectionPrompt,
            0,
            cardsToScry.Count
        );

        var cardsToDiscard = (await CardSelectCmd.FromSimpleGrid(
            choiceContext,
            cardsToScry,
            player,
            prefs
        )).ToList();
       
        foreach (var card in cardsToDiscard)
        {
            await CardPileCmd.Add(card, discardPile);
            CombatManager.Instance.History.CardDiscarded(combatState, card);
            await Hook.AfterCardDiscarded(combatState, choiceContext, card);
        }
        discardPile.InvokeContentsChanged();
        
        await WatcherHook.AfterScryed(choiceContext, player, modifiedAmount, cardsToDiscard.Count, cardsToDiscard);
        return new ScryResult(cardsToDiscard);
    }
}