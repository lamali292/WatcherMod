using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Relics;
using Watcher.Code.Character;
using Watcher.Code.Extensions;
using Watcher.Code.Powers;

namespace Watcher.Code.Relics;

[Pool(typeof(WatcherRelicPool))]
public sealed class Duality : CustomRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;
    public override string PackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".RelicImagePath();


    public override async Task AfterAttack(AttackCommand command)
    {
        if (command.Attacker != Owner.Creature)
            return;
        await PowerCmd.Apply<DualityPower>(Owner.Creature, 1, Owner.Creature, null);
    }
}