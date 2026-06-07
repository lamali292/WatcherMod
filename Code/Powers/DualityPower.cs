using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Models.Powers;
using Watcher.Code.Extensions;
using Watcher.Code.Relics;

namespace Watcher.Code.Powers;

public class DualityPower : CustomTemporaryPowerModelWrapper<Duality, DexterityPower>
{
    /*
    protected override Func<PlayerChoiceContext, Creature, decimal, Creature?, CardModel?, bool, Task> ApplyPowerFunc =>
        PowerCmd.Apply<DexterityPower>;

    public override PowerModel InternallyAppliedPower => ModelDb.Power<DexterityPower>();
    public override AbstractModel OriginModel => ModelDb.Relic<Duality>();
    */
    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    public override string CustomBigIconPath => CustomPackedIconPath;
}