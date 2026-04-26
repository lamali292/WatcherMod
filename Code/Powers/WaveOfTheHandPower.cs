using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Watcher.Code.Abstract;

namespace Watcher.Code.Powers;

public sealed class WaveOfTheHandPower : WatcherPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    public override async Task AfterBlockGained(Creature creature, decimal amount, ValueProp props,
        CardModel? cardSource)
    {
        if (creature != Owner)
            return;
        
        var weakAmount = Amount; 
        if (weakAmount <= 0)
            return;
        
        await PowerCmd.Apply<WeakPower>(
            CombatState.HittableEnemies,
            weakAmount,
            Owner,
            null
        );
    }


    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (Owner.IsAlive) RemoveInternal();
        await Task.CompletedTask;
    }
}