using BaseLib.Abstracts;
using BaseLib.Utils;
using Watcher.Code.Character;

namespace Watcher.Code.Potions;

[Pool(typeof(WatcherPotionPool))]
public abstract class WatcherPotion : CustomPotionModel;