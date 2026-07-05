using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Combat;
using Watcher.Code.Nodes;

namespace Watcher.Code.Patches;



[HarmonyPatch(typeof(NCreature), nameof(NCreature.SetAnimationTrigger))]
public static class WatcherAnimationPatch
{
    [HarmonyPrefix]
    private static bool MyAnimations(NCreature __instance, string trigger)
    {
        if (__instance.Visuals is not WatcherNCreatureVisuals hexVisuals ||
            __instance._spineAnimator == null) return true;
        hexVisuals.Animator = __instance._spineAnimator;
        hexVisuals.OnAnimationTrigger(trigger);
        return false;
    }
}

[HarmonyPatch(typeof(NCreature), nameof(NCreature.StartDeathAnim))]
public static class WatcherDeathAnimPatch
{
    [HarmonyPrefix]
    private static bool MyDeathAnimation(NCreature __instance)
    {
        if (__instance.Visuals is not WatcherNCreatureVisuals hexVisuals ||
            __instance._spineAnimator == null) return true;
        hexVisuals.Animator = __instance._spineAnimator;
        hexVisuals.OnAnimationTrigger("Dead");
        return false;
    }
}