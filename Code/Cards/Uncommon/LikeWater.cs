using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Watcher.Code.Cards.CardModels;
using Watcher.Code.Character;
using Watcher.Code.Extensions;
using Watcher.Code.Powers;
using Watcher.Code.Stances;

namespace Watcher.Code.Cards.Uncommon;

[Pool(typeof(WatcherCardPool))]
public sealed class LikeWater() : WatcherCardModel(1, CardType.Power, CardRarity.Uncommon, TargetType.None)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<LikeWaterPower>(5)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<LikeWaterPower>(),
        HoverTipFactory.Static(StaticHoverTip.Block),
        HoverTipFactory.FromPower<CalmStance>()
    ];


    

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<LikeWaterPower>(Owner.Creature, DynamicVars["LikeWaterPower"].BaseValue, Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["LikeWaterPower"].UpgradeValueBy(2m);
    }
}