using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Watcher.Code.Abstract;
using Watcher.Code.Extensions;

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
        await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, player);
        DynamicVars.Energy.UpgradeValueBy(Amount);
        Flash();
    }
}