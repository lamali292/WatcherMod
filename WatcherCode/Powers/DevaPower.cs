using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using Watcher.WatcherCode.Vfx;
using Watcher.Code.Abstract;

namespace Watcher.Code.Powers;

public sealed class DevaPower : WatcherPowerModel
{
    public override PowerType Type => PowerType.Buff;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(1)
    ];

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterEnergyReset(Player player)
    {
        if (player.Creature != Owner)
            return;

        // Give energy
        Vfx?.OnEffectTriggered();
        await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, player);
        DynamicVars.Energy.UpgradeValueBy(Amount);
        Flash();
    }

    // Same pattern as the base game's form powers (e.g. DemonFormPower): the idle VFX is created when the power is
    // applied and switched off when it is removed. A destroyed node (combat over) reads as null.
    private NDevaFormVfx? Vfx
    {
        get => field == null || field.IsValid() ? field : null;
        set
        {
            AssertMutable();
            field = value;
        }
    }

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        // Stacking applies the power again; keep the one VFX instead of adding another.
        Vfx ??= NDevaFormVfx.Create(Owner);
        return Task.CompletedTask;
    }

    public override Task AfterRemoved(Creature oldOwner)
    {
        Vfx?.SetActive(false);
        return Task.CompletedTask;
    }
}
