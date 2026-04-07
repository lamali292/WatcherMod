using MegaCrit.Sts2.Core.Entities.Players;
using Watcher.Code.Core;
using Watcher.Code.Stances;

namespace Watcher.Code.Extensions;

public static class PlayerExtensions
{
    public static WatcherStanceModel WatcherStance(this Player player)
    {
        return WatcherModel.GetStanceModel(player);
    }

    public static bool IsInWatcherStance<T>(this Player player)
        where T : WatcherStanceModel
    {
        return WatcherModel.IsInStance<T>(player);
    }
}