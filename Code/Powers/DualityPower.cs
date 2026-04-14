using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using Watcher.Code.Relics;

namespace Watcher.Code.Powers;

#pragma warning disable STS003
public class DualityPower : TemporaryDexterityPower
#pragma warning restore STS003
{
    public override AbstractModel OriginModel => ModelDb.Relic<Duality>();
}