using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Watcher.Code.Abstract;
using Watcher.Code.Character;
using Watcher.Code.Powers;

namespace Watcher.Code.Cards.Rare;

[Pool(typeof(WatcherCardPool))]
public sealed class Devotion : WatcherCardModel
{
    public Devotion() : base(1, CardType.Power, CardRarity.Rare, TargetType.None)
    {
        WithPower<DevotionPower>(2, 1);
        WithTip(typeof(MantraPower));
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<DevotionPower>(this);
    }
}