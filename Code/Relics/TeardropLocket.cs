using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using Watcher.Code.Abstract;
using Watcher.Code.Character;
using Watcher.Code.Commands;
using Watcher.Code.Core;
using Watcher.Code.Stances;

namespace Watcher.Code.Relics;

[Pool(typeof(WatcherRelicPool))]
public sealed class TeardropLocket : WatcherRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;


    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        WatcherHoverTipFactory.FromStance<CalmStance>()
    ];

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext ctx, ICombatState combatState)
    {
        if (player != Owner || Owner.PlayerCombatState is not { TurnNumber: 1 }) return;
        await StanceCmd.EnterCalm(ctx, Owner, null);
        Flash();
    }
}