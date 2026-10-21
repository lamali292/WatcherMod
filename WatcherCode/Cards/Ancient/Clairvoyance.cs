using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Watcher.Code.Abstract;
using Watcher.Code.Character;
using Watcher.Code.Powers;

namespace Watcher.Code.Cards.Ancient;

// Darv's offering (not an Orobas upgrade): Scry engine, so the two Ancient cards don't both revolve around stances.
[Pool(typeof(WatcherCardPool))]
public sealed class Clairvoyance : WatcherCardModel
{
    public Clairvoyance() : base(2, CardType.Power, CardRarity.Ancient, TargetType.None)
    {
        WithPower<ClairvoyancePower>(1, false);
        WithTip(BaseLibTip.Scry);
        WithCostUpgradeBy(-1);
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<ClairvoyancePower>(ctx, this);
    }
}
