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
        var numCards = DynamicVars.Cards.IntValue;
        var prefs = new CardSelectorPrefs(
            SelectionScreenPrompt,
            numCards,
            numCards
        )
        {
            Cancelable = false,
            PretendCardsCanBePlayed = true
        };
        var selectedCards = await CardSelectCmd.FromCombatPile(
            ctx,
            PileType.Discard.GetPile(Owner),
            Owner,
            prefs
        );

        foreach (var card in selectedCards.ToList())
        {
            await CardPileCmd.Add(card, PileType.Hand);

            card.GiveSingleTurnRetain();
        }

        await StanceCmd.EnterCalm(ctx, Owner, cardPlay.Card);
        PlayerCmd.EndTurn(Owner, false);
    }
}