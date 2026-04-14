using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using Watcher.Code.Character;
using Watcher.Code.Extensions;
using Watcher.Code.Powers;

namespace Watcher.Code.Relics;

[Pool(typeof(WatcherRelicPool))]
public sealed class Damaru : CustomRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Common;

    public override string PackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".RelicImagePath();


    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        var damaru = this;
        if (side != damaru.Owner.Creature.Side)
            return;
        await PowerCmd.Apply<MantraPower>(Owner.Creature, 1, Owner.Creature, null);
        damaru.Flash();
    }
}