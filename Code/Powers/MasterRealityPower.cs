using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using Watcher.Code.Extensions;

namespace Watcher.Code.Powers;

public sealed class MasterRealityPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;
    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    public override string CustomBigIconPath => CustomPackedIconPath;

    public override Task AfterCardGeneratedForCombat(CardModel card, bool addedByPlayer)
    {
        // TODO : is this correct. What if both players have this power?. Does it get trigger double?
        if (!card.Owner.Creature.HasPower<MasterRealityPower>())
            return Task.CompletedTask;
        if (card is { IsUpgradable: true, IsUpgraded: false }) CardCmd.Upgrade(card);
        return Task.CompletedTask;
    }
}