using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Watcher.Code.Abstract;
using Watcher.Code.Cards.Rare;

namespace Watcher.Code.Powers;

public class DevotionPower : WatcherPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;



    public override async Task BeforeHandDrawLate(Player player, PlayerChoiceContext choiceContext,
        CombatState combatState)
    {
        if (player != Owner.Player)
            return;

        await PowerCmd.Apply<MantraPower>(player.Creature, Amount, player.Creature, ModelDb.Card<Devotion>());
        Flash();
    }
}