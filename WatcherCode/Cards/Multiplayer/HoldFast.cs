using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Watcher.Code.Abstract;
using Watcher.Code.Character;
using Watcher.Code.Extensions;

namespace Watcher.Code.Cards.Multiplayer;

[Pool(typeof(WatcherCardPool))]
public class HoldFast : WatcherCardModel
{
    public HoldFast() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyAlly)
    {
        WithTip(CardKeyword.Retain);
        WithKeyword(CardKeyword.Exhaust, UpgradeType.Remove);
    }

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var target = cardPlay.Target?.Player;
        if (target == null) return;
        foreach (var card in target.Hand)
            card.GiveSingleTurnRetain();
        await Task.CompletedTask;
    }
}
