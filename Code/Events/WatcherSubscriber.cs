using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using Watcher.Code.Core;
using Watcher.Code.Stances;

namespace Watcher.Code.Events;

public class WatcherSubscriber
{
    public static void Subscribe()
    {
        ModHelper.SubscribeForCombatStateHooks(WatcherMainFile.ModId, CollectModels2);
    }

    private static IEnumerable<AbstractModel> CollectModels2(CombatState combatState)
    {
        return combatState.Players
            .Select(WatcherModel.GetStanceModel)
            .Where(s => s is not NoStance);
    }
}