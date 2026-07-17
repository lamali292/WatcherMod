using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Watcher.Code.Abstract;
using Watcher.Code.Character;
using Watcher.Code.Keywords;
using Watcher.Code.Powers;

namespace Watcher.Code.Cards.Uncommon;

[Pool(typeof(WatcherCardPool))]
public sealed class Foresight : WatcherCardModel
{
    public Foresight() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.None)
    {
        WithPower<ForesightPower>(3, 1, false);
        WithTip(WatcherKeywords.Scry);
    }

    // TODO : Make it work for multiplayer seamlessly
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.SingleplayerOnly;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<ForesightPower>(ctx, this);
    }
}