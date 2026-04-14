using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Watcher.Code.Cards.CardModels;

public abstract class WatcherCardModel(
    int canonicalEnergyCost,
    CardType type,
    CardRarity rarity,
    TargetType targetType,
    bool shouldShowInCardLibrary = true)
    : CustomCardModel(canonicalEnergyCost, type, rarity, targetType, shouldShowInCardLibrary)
{
    public virtual Task OnStanceChanged(Creature creature)
    {
        return Task.CompletedTask;
    }

    public virtual Task OnScryed(Player player, int amount)
    {
        return Task.CompletedTask;
    }
}