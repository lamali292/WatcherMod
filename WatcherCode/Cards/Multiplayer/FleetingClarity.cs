using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Watcher.Code.Abstract;
using Watcher.Code.Character;
using Watcher.Code.Commands;
using Watcher.Code.Extensions;
using Watcher.Code.Powers;
using Watcher.Code.Stances;

namespace Watcher.Code.Cards.Multiplayer;

[Pool(typeof(WatcherCardPool))]
public class FleetingClarity : WatcherCardModel
{
    public FleetingClarity() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.AllAllies)
    {
        WithPower<MultiplayerCardUncommonPower>(1, false);
        WithStanceTip<CalmStance>();
        WithKeyword(CardKeyword.Exhaust, UpgradeType.Remove);
    }

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        var players = Owner.AllTeammates;
        await CommonActions.Apply<MultiplayerCardUncommonPower>(ctx, players.Select(e => e.Creature), this, true);
        await StanceCmd.EnterCalm(ctx, players, this);
    }
}