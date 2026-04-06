using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Watcher.Code.Stances;

namespace Watcher.Code.Events;

public interface IOnStanceChange
{
    Task OnStanceChange(PlayerChoiceContext ctx, Player player, WatcherStanceModel oldStance, WatcherStanceModel newStance);
}