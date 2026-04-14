using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using Watcher.Code.Commands;
using Watcher.Code.Extensions;

namespace Watcher.Code.Powers;

public sealed class MentalFortressPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        await base.AfterApplied(applier, cardSource);
        StanceCmd.StanceChanged += OnStanceChanged;
    }

    public override async Task AfterRemoved(Creature owner)
    {
        await base.AfterRemoved(owner);
        StanceCmd.StanceChanged -= OnStanceChanged;
    }

    public override async Task AfterCombatEnd(CombatRoom room)
    {
        await base.AfterCombatEnd(room);
        StanceCmd.StanceChanged -= OnStanceChanged;
    }

    private async Task OnStanceChanged(Creature creature, PlayerChoiceContext? context)
    {
        if (creature != Owner)
            return;

        await CreatureCmd.GainBlock(
            Owner,
            Amount,
            ValueProp.Unpowered,
            null
        );
    }
}