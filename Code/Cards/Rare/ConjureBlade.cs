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
public sealed class ConjureBlade() : WatcherCardModel(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override HashSet<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];


    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Expunger>()
    ];

    protected override bool HasEnergyCostX => true;

    

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        // X = energy spent
        var x = ResolveEnergyXValue();
        if (IsUpgraded)
            x += 1;
        // Create Insight card
        var expunger = CombatState?.CreateCard<Expunger>(Owner);
        if (expunger == null)
            return;

        expunger.DynamicVars.Repeat.UpgradeValueBy(x + 1);
        // Shuffle it into draw pile (Random position)
        CardCmd.PreviewCardPileAdd(
            await CardPileCmd.AddGeneratedCardToCombat(
                expunger,
                PileType.Draw,
                true,
                CardPilePosition.Random
            )
        );
    }
}