using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using Watcher.Code.Abstract;

namespace Watcher.Code.Powers;

public sealed class MasterRealityPower : WatcherPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;


    public override Task AfterCardGeneratedForCombat(CardModel card, bool addedByPlayer)
    {
        if (card.Owner.Creature != Owner || !card.Owner.Creature.HasPower<MasterRealityPower>()) return Task.CompletedTask;
        if (card is { IsUpgradable: true, IsUpgraded: false }) CardCmd.Upgrade(card);
        return Task.CompletedTask;
    }
}