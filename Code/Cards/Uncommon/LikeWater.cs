using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using Watcher.Code.Abstract;
using Watcher.Code.Character;
using Watcher.Code.Powers;
using Watcher.Code.Stances;

namespace Watcher.Code.Cards.Uncommon;

[Pool(typeof(WatcherCardPool))]
public sealed class LikeWater : WatcherCardModel
{
    public LikeWater() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.None)
    {
        WithPower<LikeWaterPower>(5, 2);
        WithStanceTip<CalmStance>();
        WithTip(StaticHoverTip.Block);
    }


    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<LikeWaterPower>(this);
    }
}