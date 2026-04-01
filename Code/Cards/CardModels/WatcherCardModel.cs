using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using Watcher.Code.Extensions;

namespace Watcher.Code.Cards.CardModels;

public abstract class WatcherCardModel(
    int canonicalEnergyCost,
    CardType type,
    CardRarity rarity,
    TargetType targetType,
    bool shouldShowInCardLibrary = true)
    : CustomCardModel(canonicalEnergyCost, type, rarity, targetType, shouldShowInCardLibrary)
{
    public sealed override string CustomPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
}