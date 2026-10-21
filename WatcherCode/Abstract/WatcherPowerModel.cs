using BaseLib.Abstracts;
using BaseLib.Extensions;
using Watcher.Code.Extensions;

namespace Watcher.Code.Abstract;

public abstract class WatcherPowerModel : CustomPowerModel
{
    public sealed override string CustomPackedIconPath =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.tres".PowerImagePath();

    public sealed override string CustomBigIconPath =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
}