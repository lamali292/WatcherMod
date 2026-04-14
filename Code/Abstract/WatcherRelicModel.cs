using BaseLib.Abstracts;
using BaseLib.Extensions;
using Watcher.Code.Extensions;

namespace Watcher.Code.Abstract;

public abstract class WatcherRelicModel : CustomRelicModel
{
    protected override string BigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigRelicImagePath();
    public override string PackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.tres".TresRelicImagePath();

    protected override string PackedIconOutlinePath =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.tres".TresRelicImagePath();

}