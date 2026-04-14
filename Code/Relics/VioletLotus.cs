using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using Watcher.Code.Abstract;
using Watcher.Code.Character;
using Watcher.Code.Events;

namespace Watcher.Code.Relics;

[Pool(typeof(WatcherRelicPool))]
public sealed class VioletLotus : WatcherRelicModel, IModifyCalmEnergyGain
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public int ModifyCalmEnergyGain(Player player, int amount) => 
        Owner == player ? amount + 1 : amount;
}