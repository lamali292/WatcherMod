using BaseLib.Patches.Localization;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.ValueProps;
using Watcher.Code.Abstract;

namespace Watcher.Code.Powers;

public sealed class BlockReturnPower : WatcherPowerModel, IAddDumbVariablesToPowerDescription
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerInstanceType InstanceType => PowerInstanceType.InstancedPerApplier;


    private bool _applierIsAttacking;
    public override Task BeforeAttack(AttackCommand command)
    {
        if (command.Attacker == Applier)
            _applierIsAttacking = true;
        return Task.CompletedTask;
    }

 
    public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
    {
        if (Applier == null || wasRemovalPrevented || !_applierIsAttacking || creature != Owner) return;
        await CreatureCmd.GainBlock(Applier, Amount, ValueProp.Unpowered, null);
    }

    public override async Task AfterAttack(PlayerChoiceContext choiceContext, AttackCommand command)
    {
        if (Applier == null || command.Attacker != Applier) return;
        _applierIsAttacking = false;
        var count = command.Results.SelectMany(s => s).Count(e => e.Receiver == Owner);
        if (count == 0) return;
        for (var i = 0; i < count; i++)
            await CreatureCmd.GainBlock(Applier, Amount, ValueProp.Unpowered, null);
    }

    public void AddDumbVariablesToPowerDescription(LocString description)
    {
        description.Add("IsApplierYou", LocalContext.IsMe(Applier));
    }
}