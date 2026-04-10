using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using Watcher.Code.Abstract;
using Watcher.Code.Commands;
using Watcher.Code.Events;

namespace Watcher.Code.Powers;

public sealed class NirvanaPower : WatcherPowerModel, IOnScryed
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
   
    public async Task OnScryed(PlayerChoiceContext ctx, Player player, int amount, int discardedAmount)
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