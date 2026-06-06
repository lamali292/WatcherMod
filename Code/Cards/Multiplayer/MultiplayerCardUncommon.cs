using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Watcher.Code.Abstract;
using Watcher.Code.Character;
using Watcher.Code.Commands;
using Watcher.Code.Powers;
using Watcher.Code.Stances;

namespace Watcher.Code.Cards.Multiplayer;

[Pool(typeof(WatcherCardPool))]
public class MultiplayerCardUncommon : WatcherCardModel
{
    public MultiplayerCardUncommon() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.AllAllies)
    {
        WithPower<MultiplayerCardUncommonPower>(1, false);
        WithStanceTip<CalmStance>();
        WithKeyword(CardKeyword.Exhaust, UpgradeType.Remove);
    }

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        var players = CombatState.PlayerCreatures.Where(c => c.IsAlive && c != Owner.Creature).ToList();
        await CommonActions.Apply<MultiplayerCardUncommonPower>(ctx, players, this, true);
        foreach (var player in players.Select(e => e.Player))
        {
            if (player == null) continue;
            await StanceCmd.EnterCalm(ctx, player, this);
        }
    }
}