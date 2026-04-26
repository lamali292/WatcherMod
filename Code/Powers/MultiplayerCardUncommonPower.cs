using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using Watcher.Code.Abstract;
using Watcher.Code.Commands;
using Watcher.Code.Core;
using Watcher.Code.Extensions;
using Watcher.Code.Stances;

namespace Watcher.Code.Powers;

public class MultiplayerCardUncommonPower : WatcherPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        WatcherHoverTipFactory.FromStance<CalmStance>()
    ];

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext ctx, CombatState combatState)
    {
        if (player.Creature != Owner) return;
        if (player.IsInWatcherStance<CalmStance>())
        {
            await StanceCmd.ExitStance(ctx, player, null);
        }
        RemoveInternal();
    }
    

}
