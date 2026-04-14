using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using Watcher.Code.Abstract;
using Watcher.Code.Extensions;
using Watcher.Code.Stances;

namespace Watcher.Code.Powers;

public class LikeWaterPower : WatcherPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;


    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
    if (Owner.Player?.Creature.Side != side) return;

        var isInCalm = Owner.Powers.OfType<CalmStance>().Any();

        if (isInCalm)
        {
            await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Unpowered, null);
            Flash();
        }
    }
}