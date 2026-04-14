using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using Watcher.Code.Commands;
using Watcher.Code.Extensions;

namespace Watcher.Code.Powers;

public sealed class MantraPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        // Subscribe to the Owner's PowerIncreased event
        Owner.PowerIncreased += OnPowerIncreased;

        // Check immediately in case we start with 10+
        await CheckForDivinity();
    }

    public override Task AfterRemoved(Creature oldOwner)
    {
        // Unsubscribe when removed
        oldOwner.PowerIncreased -= OnPowerIncreased;
        return Task.CompletedTask;
    }

    private async void OnPowerIncreased(PowerModel power, int change, bool silent)
    {
        // Only react to our own increases
        if (power == this && change > 0) await CheckForDivinity();
    }

    private async Task CheckForDivinity()
    {
        // Check if we have 10 or more Mantra
        while (Amount >= 10)
        {
            var player = Owner.Player;
            if (player != null)
            {
                await StanceCmd.EnterDivinity(player.Creature, null);
                await PowerCmd.ModifyAmount(this, -10m, null, null);
            }
            else
            {
                break;
            }
        }
    }
}