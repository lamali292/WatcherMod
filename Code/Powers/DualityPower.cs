using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using Watcher.Code.Relics;

namespace Watcher.Code.Powers;

public class DualityPower : TemporaryDexterityPower
{
    public override AbstractModel OriginModel => ModelDb.Relic<Duality>();
}