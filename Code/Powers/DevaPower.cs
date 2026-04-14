using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Watcher.Code.Extensions;

namespace Watcher.Code.Powers;

public sealed class DevaPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(1)
    ];

    public override PowerStackType StackType => PowerStackType.Counter;
    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();


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