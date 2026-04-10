using MegaCrit.Sts2.Core.Entities.Players;

namespace Watcher.Code.Events;

public interface IModifyCalmEnergyGain
{
    int ModifyCalmEnergyGain(Player player, int amount);
}