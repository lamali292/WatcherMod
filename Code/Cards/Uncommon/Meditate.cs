using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Watcher.Code.Abstract;
using Watcher.Code.Character;
using Watcher.Code.Commands;
using Watcher.Code.Stances;

namespace Watcher.Code.Cards.Uncommon;

[Pool(typeof(WatcherCardPool))]
public sealed class Meditate : WatcherCardModel
{
    public Meditate() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithCards(1, 1);
        WithTip(CardKeyword.Retain);
        WithStanceTip<CalmStance>();
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var discardPile = PileType.Discard.GetPile(Owner);
        var cardsInDiscard = discardPile.Cards.ToList();
        if (cardsInDiscard.Count != 0)
        {
            var numCards = DynamicVars.Cards.IntValue;
            var maxSelect = Math.Min(numCards, cardsInDiscard.Count);
            var prefs = new CardSelectorPrefs(
                SelectionScreenPrompt,
                maxSelect,
                maxSelect
            )
            {
                Cancelable = false,
                PretendCardsCanBePlayed = true
            };
            var selectedCards = await CardSelectCmd.FromSimpleGrid(
                ctx,
                cardsInDiscard,
                Owner,
                prefs
            );

            foreach (var card in selectedCards)
            {
                await CardPileCmd.Add(card, PileType.Hand);

                card.GiveSingleTurnRetain();
            }
        }

        await StanceCmd.EnterCalm(ctx, Owner, cardPlay.Card);
        PlayerCmd.EndTurn(Owner, false);
    }
}