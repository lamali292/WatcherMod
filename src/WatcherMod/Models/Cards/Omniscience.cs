using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace WatcherMod.Models.Cards;

public sealed class Omniscience() : CardModel(4, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override HashSet<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // Select a card from your draw pile
        var source = this;
        await CreatureCmd.TriggerAnim(source.Owner.Creature, "Cast", source.Owner.Character.CastAnimDelay);
        var prefs = new CardSelectorPrefs(source.SelectionScreenPrompt, 1)
        {
            PretendCardsCanBePlayed = true
        };
        var drawPile = PileType.Draw.GetPile(Owner).Cards.ToList();
        var card = (await CardSelectCmd.FromSimpleGrid(
            choiceContext,
            drawPile,
            Owner,
            prefs
        )).FirstOrDefault();


        if (card != null)
        {
            await CardCmd.AutoPlay(choiceContext, card, null);
            if (card.Type == CardType.Power) card.HasBeenRemovedFromState = false;
            await CardCmd.AutoPlay(choiceContext, card, null);
            if (card.Type != CardType.Power) await CardCmd.Exhaust(choiceContext, card);
        }
    }


    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}