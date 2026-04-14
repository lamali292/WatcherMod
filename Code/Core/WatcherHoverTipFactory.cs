using MegaCrit.Sts2.Core.HoverTips;
using Watcher.Code.Stances;

namespace Watcher.Code.Core;

public class WatcherHoverTipFactory
{
    public static IHoverTip FromStance<T>() where T : WatcherStanceModel
    {
        return FromStance(WatcherModelDb.WatcherStance<T>());
    }

    public static IHoverTip FromStance(WatcherStanceModel model) => model.DumbHoverTip;

}