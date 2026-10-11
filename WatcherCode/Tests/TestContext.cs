using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Encounters;

namespace Watcher.Code.Tests;

public class TestContext
{
    public CombatState Combat { get; }
    /// The local player (net id 1).
    public Player Player { get; }
    /// Every player in the combat, local first. Only has more than one entry for tests declared with playerCount > 1.
    public IReadOnlyList<Player> Players { get; }

    public TestContext(CombatState combat, IReadOnlyList<Player> players)
    {
        Combat = combat;
        Players = players;
        Player = players[0];
    }

    public Task<CardModel> AddCardToHand<T>() where T : CardModel => AddCardToHand<T>(Player);

    public async Task<CardModel> AddCardToHand<T>(Player player) where T : CardModel
    {
        var card = Combat.CreateCard(ModelDb.Card<T>(), player);
        await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, player);
        return card;
    }


    public async Task<CardModel> AddCardToTopOfDraw<T>() where T : CardModel
    {
        var card = Combat.CreateCard(ModelDb.Card<T>(), Player);
        await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Draw, Player, CardPilePosition.Top);
        return card;
    }

    public async Task PlayCard(CardModel card, Creature? target = null)
    {
        await CardCmd.AutoPlay(new BlockingPlayerChoiceContext(), card, target);
    }

    /// Empties a player's hand (defaulting to the local player) to the discard pile.
    public async Task ClearHand(Player? player = null)
    {
        player ??= Player;
        var hand = PileType.Hand.GetPile(player).Cards.ToList();
        if (hand.Count > 0) await CardPileCmd.Add(hand, PileType.Discard);
    }
}