using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using Watcher.Code.Commands;
using Watcher.Code.Extensions;
using Watcher.Code.Stances;
using Watcher.Code.Stances.Vfx;

namespace Watcher.Code.Powers;

public sealed class MantraPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    public override string CustomBigIconPath => CustomPackedIconPath;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<DivinityStance>()
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