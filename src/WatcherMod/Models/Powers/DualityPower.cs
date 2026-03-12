using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using WatcherMod.Relics;

namespace WatcherMod.Models.Powers;

public class DualityPower : TemporaryDexterityPower
{
    public override AbstractModel OriginModel => ModelDb.Relic<Duality>();
}