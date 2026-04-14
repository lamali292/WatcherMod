using MegaCrit.Sts2.Core.Entities.Players;

namespace Watcher.Code.Events;

public interface IModifyWrathDamage
{
    decimal ModifyWrathDamage(Player player, decimal multiplier);
}