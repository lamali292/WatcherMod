using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using WatcherMod.Models.Cards.CardModels;
using WatcherMod.Relics;

namespace WatcherMod.Commands;

public static class ScryCmd
{
    public static event Func<Player, int, Task>? Scryed;

    public static async Task Execute(PlayerChoiceContext choiceContext, Player player, int amount)
    {
        if (player.GetRelic<GoldenEye>() != null) amount += 2;

        if (amount <= 0) return;

        var drawPile = PileType.Draw.GetPile(player);
        var cardsToScry = drawPile.Cards.Take(amount).ToList();


        if (!cardsToScry.Any()) return;
        var prefs = new CardSelectorPrefs(
            CardSelectorPrefs.DiscardSelectionPrompt,
            0,
            cardsToScry.Count
        );

        var cardsToDiscard = await CardSelectCmd.FromSimpleGrid(
            choiceContext,
            cardsToScry,
            player,
            prefs
        );
        foreach (var card in cardsToDiscard) await CardPileCmd.Add(card, PileType.Discard);

        await NotifyPowers(player, amount);
        await NotifyCards(player, amount);
    }


    private static async Task NotifyPowers(Player player, int amount)
    {
        if (Scryed != null) await Scryed.Invoke(player, amount);
    }

    private static async Task NotifyCards(Player? player, int amount)
    {
        if (player?.PlayerCombatState?.AllPiles != null)
            foreach (var pile in player.PlayerCombatState?.AllPiles!)
            {
                var watcherCards = pile.Cards.OfType<WatcherCardModel>().ToList();

                foreach (var card in watcherCards) await card.OnScryed(player, amount);
            }
    }
}