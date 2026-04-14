using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Relics;
using Watcher.Code.Character;
using Watcher.Code.Commands;
using Watcher.Code.Extensions;

namespace Watcher.Code.Relics;

[Pool(typeof(WatcherRelicPool))]
public sealed class TeardropLocket : CustomRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    public override string PackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".RelicImagePath();


    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        var locket = this;
        if (side != locket.Owner.Creature.Side || combatState.RoundNumber > 1)
            return;

        await StanceCmd.EnterCalm(Owner.Creature, null);
        locket.Flash();
    }
}