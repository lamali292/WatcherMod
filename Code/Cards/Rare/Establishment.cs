using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Watcher.Code.Abstract;
using Watcher.Code.Character;
using Watcher.Code.Powers;

namespace Watcher.Code.Cards.Rare;

[Pool(typeof(WatcherCardPool))]
public sealed class Establishment : WatcherCardModel
{
    public Establishment() : base(1, CardType.Power, CardRarity.Rare, TargetType.None)
    {
        WithPower<EstablishmentPower>(1, false);
        WithTip(CardKeyword.Retain);
        WithKeyword(CardKeyword.Innate, UpgradeType.Add);
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<EstablishmentPower>(ctx, this);
    }
}