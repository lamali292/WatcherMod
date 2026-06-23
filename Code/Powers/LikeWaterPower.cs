using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.ValueProps;
using Watcher.Code.Abstract;
using Watcher.Code.Core;
using Watcher.Code.Extensions;
using Watcher.Code.Stances;

namespace Watcher.Code.Powers;

public class LikeWaterPower : WatcherPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(StaticHoverTip.Block),
        WatcherHoverTipFactory.FromStance<CalmStance>()
    ];


    public override async Task BeforeSideTurnEndEarly(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (Owner.Player?.Creature.Side != side) return;
        var isInCalm = Owner.Player.IsInWatcherStance<CalmStance>();
        if (isInCalm)
        {
            await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Unpowered, null);
            Flash();
        }
    }
}