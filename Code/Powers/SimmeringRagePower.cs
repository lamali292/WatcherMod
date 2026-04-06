using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Watcher.Code.Abstract;
using Watcher.Code.Cards.Uncommon;
using Watcher.Code.Commands;

namespace Watcher.Code.Powers;

public sealed class SimmeringRagePower : WatcherPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext ctx, CombatState combatState)
    {
        if (!player.Creature.HasPower<SimmeringRagePower>())
            return;
        await StanceCmd.EnterWrath(ctx, player, ModelDb.Card<SimmeringFury>());
        await PowerCmd.TickDownDuration(this);
    }
}