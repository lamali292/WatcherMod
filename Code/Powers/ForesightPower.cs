using BaseLib.Commands;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using Watcher.Code.Abstract;

namespace Watcher.Code.Powers;

public class ForesightPower : WatcherPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(BaseLibTip.Scry)];
    
    public override async Task BeforeHandDrawLate(Player player, PlayerChoiceContext choiceContext,
        ICombatState combatState)
    {
        if (player != Owner.Player)
            return;
        await ScryCmd.Execute(new BlockingPlayerChoiceContext(), player, Amount);
    }
}