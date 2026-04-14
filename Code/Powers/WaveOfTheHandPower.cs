using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Watcher.Code.Extensions;

namespace Watcher.Code.Powers;

public sealed class WaveOfTheHandPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();

    public override async Task AfterBlockGained(Creature creature, decimal amount, ValueProp props,
        CardModel? cardSource)
    {
        // Only trigger for the owner of this power
        if (creature != Owner)
            return;

        // Apply Weak to all enemies
        var weakAmount = Amount; // Use power stacks
        if (weakAmount <= 0)
            return;

        await PowerCmd.Apply<WeakPower>(
            CombatState.HittableEnemies, // all enemies
            weakAmount,
            Owner, // source is the creature with this power
            null
        );
    }


    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (Owner.IsAlive) RemoveInternal();
        await Task.CompletedTask;
    }
}