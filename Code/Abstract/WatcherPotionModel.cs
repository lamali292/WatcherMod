using BaseLib.Abstracts;
using BaseLib.Extensions;
using Watcher.Code.Extensions;

namespace Watcher.Code.Abstract;

public abstract class WatcherPotionModel : CustomPotionModel
{
    public override string CustomPackedImagePath =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.tres".PackedPotionImagePath();

    public override string CustomPackedOutlinePath =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.tres".PackedPotionImagePath();
}