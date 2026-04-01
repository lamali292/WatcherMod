using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Watcher.Code.Cards.CardModels;
using Watcher.Code.Cards.Token;
using Watcher.Code.Character;
using Watcher.Code.Extensions;

namespace Watcher.Code.Cards.Uncommon;

[Pool(typeof(WatcherCardPool))]
public sealed class DeceiveReality() : WatcherCardModel(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(4m, ValueProp.Move)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Safety>()
    ];

    

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);

        var insightCard = CombatState?.CreateCard<Safety>(Owner);

        // Add to hand at top position
        if (insightCard != null)
            CardCmd.PreviewCardPileAdd(
                await CardPileCmd.AddGeneratedCardToCombat(
                    insightCard,
                    PileType.Hand,
                    false,
                    CardPilePosition.Top
                )
            );
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
    }
}