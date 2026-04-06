using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;
using Watcher.Code.Abstract;
using Watcher.Code.Character;
using Watcher.Code.Powers;

namespace Watcher.Code.Cards.Uncommon;

[Pool(typeof(WatcherCardPool))]
public sealed class WaveOfTheHand : WatcherCardModel
{
    public WaveOfTheHand() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithPower<WaveOfTheHandPower>(1, 1);
        WithTip(typeof(WeakPower));
        WithTip(StaticHoverTip.Block);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<WaveOfTheHandPower>(this);
    }
}