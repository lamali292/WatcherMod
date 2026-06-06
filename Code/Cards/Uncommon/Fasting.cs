using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using Watcher.Code.Abstract;
using Watcher.Code.Character;
using Watcher.Code.Powers;

namespace Watcher.Code.Cards.Uncommon;

[Pool(typeof(WatcherCardPool))]
public sealed class Fasting : WatcherCardModel
{
    public Fasting() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithPower<StrengthPower>(3, 1);
        WithPower<DexterityPower>(3, 1);
        WithPower<FastingPower>(1, false);
        WithEnergyTip();
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<StrengthPower>(ctx, this);
        await CommonActions.ApplySelf<DexterityPower>(ctx, this);
        await CommonActions.ApplySelf<FastingPower>(ctx, this);
    }
}