using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Watcher.Code.Abstract;
using Watcher.Code.Character;

namespace Watcher.Code.Cards.Rare;

[Pool(typeof(WatcherCardPool))]
public sealed class Omniscience : WatcherCardModel
{
    public Omniscience() : base(4, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithKeywords(CardKeyword.Exhaust);
        WithCostUpgradeBy(-1);
    }


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
}