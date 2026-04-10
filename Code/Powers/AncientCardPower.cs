using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using Watcher.Code.Abstract;
using Watcher.Code.Events;

namespace Watcher.Code.Powers;

public class AncientCardPower : WatcherPowerModel, IModifyWrathDamage
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    public decimal ModifyWrathDamage(Player player, decimal amount)
    {
        if (player.Creature != Owner) return amount;
        return amount + Amount/100m;
    }
}