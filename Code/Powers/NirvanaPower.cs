using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using Watcher.Code.Commands;
using Watcher.Code.Extensions;

namespace Watcher.Code.Powers;

public sealed class NirvanaPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        await base.AfterApplied(applier, cardSource);
        ScryCmd.Scryed += OnScryed;
    }

    public override async Task AfterRemoved(Creature owner)
    {
        await base.AfterRemoved(owner);
        ScryCmd.Scryed -= OnScryed;
    }

    public override async Task AfterCombatEnd(CombatRoom room)
    {
        await base.AfterCombatEnd(room);
        ScryCmd.Scryed -= OnScryed;
    }

    private async Task OnScryed(Player player, int amount)
    {
        if (player != Owner.Player)
            return;

        await CreatureCmd.GainBlock(
            Owner,
            Amount,
            ValueProp.Unpowered,
            null
        );
    }
}