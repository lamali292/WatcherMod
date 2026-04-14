using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Saves.Managers;

namespace Watcher.Code.Patches;

[HarmonyPatch]
internal class ProgressSaveManager_Patches
{

    [HarmonyPatch(typeof(ProgressSaveManager))]
    [HarmonyPatch("ObtainCharUnlockEpoch")]
    [HarmonyPatch([typeof(Player), typeof(int)])]
    private static class ObtainEpochPatch
    {
        private static bool Prefix(ProgressSaveManager __instance, Player localPlayer, int act)
        {
            Console.WriteLine(
                $"[Prefix] ObtainCharUnlockEpoch started for {localPlayer.Character.GetType().Name}, Act {act + 1}");

            // Skip method for Watcher or handle custom logic
            return localPlayer.Character is not Character.Watcher;
        }

        private static void Postfix(ProgressSaveManager __instance, Player localPlayer, int act)
        {
            Console.WriteLine(
                $"[Postfix] ObtainCharUnlockEpoch finished for {localPlayer.Character.GetType().Name}, Act {act + 1}");
        }
    }
}