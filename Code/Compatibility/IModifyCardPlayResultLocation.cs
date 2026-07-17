using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace Watcher.Code.Compatibility;

public interface IModifyCardPlayResultLocation
{
    CardLocationCompatiblity ModifyCardPlayResultLocationCompability(CardModel card, bool isAutoPlay, ResourceInfo resources,
        CardLocationCompatiblity cardLocation) => cardLocation;

    Task AfterModifyingCardPlayResultLocationCompability(CardModel card, CardLocationCompatiblity cardLocation) => Task.CompletedTask;
}

public record struct CardLocationCompatiblity(Player Player, PileType PileType, CardPilePosition Position);
