using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using WatcherMod.Models.Powers;

namespace WatcherMod.Relics;

public sealed class Duality : RelicModel
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;


    public override async Task AfterAttack(AttackCommand command)
    {
        if (command.Attacker != Owner.Creature)
            return;
        await PowerCmd.Apply<DualityPower>(Owner.Creature, 1, Owner.Creature, null);
    }
}