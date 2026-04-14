using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using Watcher.Code.Abstract;
using Watcher.Code.Cards.Ancient;

namespace Watcher.Code.Powers;

public class DisciplePower : WatcherPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (Owner.Player == null) return;
        if (Owner.Side != side) return;
        if (Owner.Player.PlayerCombatState == null) return;
        var energy = Owner.Player.PlayerCombatState.Energy;
        await PowerCmd.Apply<DrawCardsNextTurnPower>(Owner, energy, Owner, ModelDb.Card<AncientCard>());
    }
}