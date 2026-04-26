using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Watcher.Code.Abstract;
using Watcher.Code.Character;
using Watcher.Code.Powers;

namespace Watcher.Code.Relics;

[Pool(typeof(WatcherRelicPool))]
public sealed class Duality : WatcherRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;


    public override async Task AfterAttack(AttackCommand command)
    {
        if (command.Attacker != Owner.Creature) return;
        await PowerCmd.Apply<DualityPower>(Owner.Creature, 1, Owner.Creature, null, true);
    }
}