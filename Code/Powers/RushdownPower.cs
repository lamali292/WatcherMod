using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Watcher.Code.Abstract;
using Watcher.Code.Extensions;
using Watcher.Code.Stances;

namespace Watcher.Code.Powers;

public sealed class RushdownPower : WatcherPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override Task AfterPowerAmountChanged(
        PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power is not WrathStance || amount <= 0 || applier != Owner || Owner.Player == null ||
            !LocalContext.IsMe(Owner.Player) || LocalContext.NetId == null)
            return Task.CompletedTask;

        var ctx = new HookPlayerChoiceContext(this, LocalContext.NetId.Value, CombatState, GameActionType.Combat);
        
        CardPileCmd.Draw(ctx, Owner.Player);
        Flash();
        return Task.CompletedTask;
    }
}