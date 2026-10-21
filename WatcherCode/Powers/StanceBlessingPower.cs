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

// Mental Fortress for the team's offence: every stance change of the owner gives a random other player Vigor.
// One random ally (combat RNG, so every client picks the same one) keeps it from scaling with the player count.
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
        var ally = player.RunState.Rng.CombatTargets.NextItem(others);
        if (ally == null) return;

        Flash();
        await PowerCmd.Apply<VigorPower>(ctx, ally.Creature, Amount, Owner, null);
    }
}
