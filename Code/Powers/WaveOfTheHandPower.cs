using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Watcher.Code.Abstract;

namespace Watcher.Code.Powers;

public sealed class WaveOfTheHandPower : WatcherPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;


    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Block)];

    public override async Task AfterBlockGained(Creature creature, decimal amount, ValueProp props,
        CardModel? cardSource)
    {
        if (creature != Owner)
            return;

        var weakAmount = Amount;
        if (weakAmount <= 0)
            return;

        await PowerCmd.Apply<WeakPower>(
            new ThrowingPlayerChoiceContext(),
            CombatState.HittableEnemies,
            weakAmount,
            Owner,
            null
        );
    }


    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (Owner.IsAlive) RemoveInternal();
        await Task.CompletedTask;
    }
}