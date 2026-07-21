using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Events.Custom;
using Watcher.Code.Nodes;

namespace Watcher.Code.Patches;

[HarmonyPatch(typeof(NFakeMerchant), "StartCharacterAnimation")]
public static class FakeMerchantAnimationPatch
{
    [HarmonyPrefix]
    static bool Prefix(NCreatureVisuals visuals)
    {
        if (visuals is not WatcherNCreatureVisuals animatedVisuals) return true;
        animatedVisuals.OnAnimationTrigger("Idle");
        return false;
    }
}