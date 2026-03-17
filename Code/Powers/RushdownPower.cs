using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using Watcher.Code.Actions;
using Watcher.Code.Extensions;
using Watcher.Code.Stances;

namespace Watcher.Code.Powers;

public sealed class RushdownPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    public override string CustomBigIconPath => CustomPackedIconPath;

    public override Task AfterPowerAmountChanged(
        PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power is not WrathStance || amount <= 0 || applier != Owner || Owner.Player == null ||
            !LocalContext.IsMe(Owner.Player))
            return Task.CompletedTask;
        RunManager.Instance.ActionQueueSynchronizer.RequestEnqueue(
            new DrawCardsAction(Owner.Player, (uint)Amount));
        Flash();
        return Task.CompletedTask;
    }
}