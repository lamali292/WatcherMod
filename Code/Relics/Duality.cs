using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using Watcher.Code.Abstract;
using Watcher.Code.Character;
using Watcher.Code.Powers;

namespace Watcher.Code.Relics;

[Pool(typeof(WatcherRelicPool))]
public sealed class Duality : WatcherRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;


    public override async Task AfterAttack(PlayerChoiceContext choiceContext, AttackCommand command)
    {
        if (command.Attacker != Owner.Creature) return;
        // Pair each attack: +1 Dexterity now, +1 DualityPower stack to subtract that Dexterity at turn end.
        await PowerCmd.Apply<DexterityPower>(choiceContext, Owner.Creature, 1, Owner.Creature, null);
        await PowerCmd.Apply<DualityPower>(choiceContext, Owner.Creature, 1, Owner.Creature, null, true);
    }
}