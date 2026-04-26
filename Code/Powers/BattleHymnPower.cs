using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Watcher.Code.Abstract;
using Watcher.Code.Cards.Token;
using Watcher.Code.Commands;

namespace Watcher.Code.Powers;

public sealed class BattleHymnPower : WatcherPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, CombatState combatState)
    {
        if (Owner.Player != player) return;
        await WatcherCmd.GiveCards<Smite>(Owner.Player, Amount, PileType.Hand, CardPilePosition.Top, skipAnimation: true);
        Flash();
    }
}
