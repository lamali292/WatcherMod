using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using Watcher.Code.Abstract;
using Watcher.Code.Commands;
using Watcher.Code.Core;
using Watcher.Code.Stances;
using Watcher.Code.Stances.Vfx;

namespace Watcher.Code.Powers;

public sealed class MantraPower : WatcherPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        WatcherHoverTipFactory.FromStance<DivinityStance>()
    ];

    public override async Task AfterPowerAmountChanged(
        PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        var player = Owner.Player;
        if (power is not MantraPower || amount <= 0 || applier != Owner || player == null)
            return;

        StanceVfx.PlayStanceSfx("res://Watcher/audio/mantra_gain.ogg");

        while (Amount >= 10)
        {
            await StanceCmd.EnterDivinity(player.Creature, cardSource);
            await PowerCmd.ModifyAmount(this, -10m, Owner, cardSource);
        }
    }
}