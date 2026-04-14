using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Watcher.Code.Extensions;

namespace Watcher.Code.Powers;

public sealed class BlockReturnPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;
    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();

    public override async Task AfterAttack(AttackCommand command)
    {
        // Check if this creature was attacked
        foreach (var result in command.Results)
            if (result.Receiver == Owner) // Owner is the creature with this power
            {
                var attacker = command.Attacker;
                if (attacker == null) continue;
                decimal blockAmount = Amount;
                if (blockAmount <= 0) continue;
                await CreatureCmd.GainBlock(attacker, blockAmount, ValueProp.Unpowered, null);
                Flash();
            }
    }
}