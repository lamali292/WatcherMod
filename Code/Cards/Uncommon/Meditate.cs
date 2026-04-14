using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Watcher.Code.Character;
using Watcher.Code.Commands;
using Watcher.Code.Extensions;
using Watcher.Code.Stances;

namespace Watcher.Code.Cards.Uncommon;

[Pool(typeof(WatcherCardPool))]
public sealed class Meditate() : CustomCardModel(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CardKeyword.Retain),
        HoverTipFactory.FromPower<CalmStance>()
    ];

    public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var discardPile = PileType.Discard.GetPile(Owner);
        var cardsInDiscard = discardPile.Cards.ToList();

        if (cardsInDiscard.Any())
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
                choiceContext,
                cardsInDiscard,
                Owner,
                prefs
            );

            // Move to hand and retain each selected card
            foreach (var card in selectedCards)
            {
                await CardPileCmd.Add(card, PileType.Hand);

                card.GiveSingleTurnRetain();
            }
        }

        // Enter Calm
        await StanceCmd.EnterCalm(Owner.Creature, choiceContext);

        // End turn
        PlayerCmd.EndTurn(Owner, false);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}