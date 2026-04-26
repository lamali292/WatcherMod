using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using Watcher.Code.Extensions;
using Watcher.Code.Relics;

namespace Watcher.Code.Powers;

public class DualityPower : CustomTemporaryPowerModel
{
    protected override Func<PlayerChoiceContext, Creature, decimal, Creature?, CardModel?, bool, Task> ApplyPowerFunc =>
        async (_, target, amount, applier, cardSource, silent) =>
        {
            await PowerCmd.Apply<DexterityPower>(target, amount, applier, cardSource, silent);
        };

    public override PowerModel InternallyAppliedPower => ModelDb.Power<DexterityPower>();
    public override AbstractModel OriginModel => ModelDb.Relic<Duality>();
    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    public override string CustomBigIconPath => CustomPackedIconPath;
}