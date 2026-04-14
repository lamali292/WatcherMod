using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Watcher.Code.Character;
using Watcher.Code.Extensions;

namespace Watcher.Code.Potions;


public abstract class WatcherPotion : CustomPotionModel
{
    public override string PackedImagePath =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.tres".PackedPotionImagePath();

    public override string PackedOutlinePath =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.tres".PackedPotionImagePath();
}