using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Watcher.Code.Events;

public interface IAfterScryed
{
    Task AfterScryed(PlayerChoiceContext ctx, Player player, int scryAmount, int discardAmount, IEnumerable<CardModel> discarded);
}   