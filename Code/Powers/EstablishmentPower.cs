using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using Watcher.Code.Abstract;

namespace Watcher.Code.Powers;

public sealed class EstablishmentPower : WatcherPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardRetained(CardModel card)
    {
        if (card.Owner.Creature != Owner) return;
        card.EnergyCost.AddThisCombat(-Amount);
        await Task.CompletedTask;
    }


    public override async Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != Owner.Side || Owner.Player == null) return;
        if (Owner.GetPower<RetainHandPower>() == null) return;
        foreach (var card in PileType.Hand.GetPile(Owner.Player).Cards)
        {
            card.EnergyCost.AddThisCombat(-Amount);
        }
        await Task.CompletedTask;
    }
}