using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Watcher.Code.Events;

public interface IOnScryed
{
    Task OnScryed(PlayerChoiceContext ctx, Player player, int scryAmount, int discardAmount);

}