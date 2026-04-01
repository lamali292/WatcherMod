using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using Watcher.Code.Cards.CardModels;
using Watcher.Code.Cards.Token;
using Watcher.Code.Character;
using Watcher.Code.Extensions;

namespace Watcher.Code.Cards.Rare;

[Pool(typeof(WatcherCardPool))]
public sealed class Alpha() : WatcherCardModel(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        HoverTipFactory.FromCardWithCardHoverTips<Beta>();


    public override HashSet<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    


    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // Create Insight card
        if (CombatState == null) return;
        var insightCard = CombatState.CreateCard<Beta>(Owner);

        // Shuffle it into draw pile (Random position)
        CardCmd.PreviewCardPileAdd(
            await CardPileCmd.AddGeneratedCardToCombat(
                insightCard,
                PileType.Draw,
                true,
                CardPilePosition.Random
            )
        );
    }


    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
    }
}