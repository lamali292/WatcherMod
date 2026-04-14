using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using Watcher.Code.Extensions;

namespace Watcher.Code.Powers;

public sealed class EstablishmentPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();

    public override async Task AfterCardRetained(CardModel card)
    {
        card.EnergyCost.AddThisCombat(-Amount);
        await Task.CompletedTask;
    }
}