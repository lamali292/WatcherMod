using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using Watcher.Code.Abstract;

namespace Watcher.Code.Powers;

#pragma warning disable STS001
public class VaultPower : WatcherPowerModel
#pragma warning restore STS001
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override bool IsVisibleInternal => false;

    public override bool ShouldTakeExtraTurn(Player player) => player.Creature == Owner;
    
    public override Task AfterTakingExtraTurn(Player player)
    {
        return player.Creature == Owner ? PowerCmd.Decrement(this) : Task.CompletedTask;
    }
}