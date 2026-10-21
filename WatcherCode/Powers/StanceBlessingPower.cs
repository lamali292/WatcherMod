using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;
using Watcher.Code.Abstract;
using Watcher.Code.Events;
using Watcher.Code.Extensions;
using Watcher.Code.Stances;

namespace Watcher.Code.Powers;

// Mental Fortress for the team's offence: every stance change of the owner gives every other player Vigor.
public sealed class StanceBlessingPower : WatcherPowerModel, IOnStanceChange
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<VigorPower>()];

    public async Task OnStanceChange(PlayerChoiceContext ctx, Player player, WatcherStanceModel oldStance,
        WatcherStanceModel newStance)
    {
        if (player != Owner.Player) return;

        var others = player.AllTeammates.Where(teammate => teammate != player).ToList();
        if (others.Count == 0) return;

        Flash();
        foreach (var other in others)
            await PowerCmd.Apply<VigorPower>(ctx, other.Creature, Amount, Owner, null);
    }
}
