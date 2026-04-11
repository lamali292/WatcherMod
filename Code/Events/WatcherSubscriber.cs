using HarmonyLib;
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

    
    public static IEnumerable<AbstractModel> CollectModels2(CombatState combatState)
    {
        return combatState.Players
            .Select(WatcherModel.GetStanceModel)
            .Where(s => s is not NoStance);
    }
}

/*

[HarmonyPatch(typeof(CombatState), nameof(CombatState.IterateHookListeners))]
public static class CombatStateHookListenersPatch
{
    static IEnumerable<AbstractModel> Postfix(
        IEnumerable<AbstractModel> __result,
        CombatState __instance)
    {
        foreach (var model in __result)
            yield return model;

        foreach (var stanceModel in WatcherSubscriber.CollectModels2(__instance))
        {
            yield return stanceModel;
        }
    }
}*/