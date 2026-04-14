using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
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
}