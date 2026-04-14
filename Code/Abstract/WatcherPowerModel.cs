using BaseLib.Abstracts;
using BaseLib.Extensions;
using Watcher.Code.Extensions;

namespace Watcher.Code.Abstract;

public abstract class WatcherPowerModel : CustomPowerModel
{
    public sealed override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    public sealed override string CustomBigIconPath => CustomPackedIconPath;

}