using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Saves.Managers;
using WatcherMod.Models.Characters;

namespace WatcherMod.Patches;

[HarmonyPatch]
internal class ProgressSaveManager_Patches
{
    // -------------------------------
    // Patch CheckFifteenElitesDefeatedEpoch
    // -------------------------------
    [HarmonyPatch(typeof(ProgressSaveManager), "CheckFifteenElitesDefeatedEpoch")]
    [HarmonyPatch(new[] { typeof(Player) })]
    private static class ElitesPatch
    {
        private static bool Prefix(ProgressSaveManager __instance, Player localPlayer)
        {
            Console.WriteLine("[Prefix] CheckFifteenElitesDefeatedEpoch started for " +
                              localPlayer.Character.GetType().Name);
            return localPlayer.Character is not Watcher; // skip original for Watcher
        }

        private static void Postfix(ProgressSaveManager __instance, Player localPlayer)
        {
            Console.WriteLine("[Postfix] CheckFifteenElitesDefeatedEpoch finished for " +
                              localPlayer.Character.GetType().Name);
        }
    }

    // -------------------------------
    // Patch CheckFifteenBossesDefeatedEpoch
    // -------------------------------
    [HarmonyPatch(typeof(ProgressSaveManager), "CheckFifteenBossesDefeatedEpoch")]
    [HarmonyPatch(new[] { typeof(Player) })]
    private static class BossesPatch
    {
        private static bool Prefix(ProgressSaveManager __instance, Player localPlayer)
        {
            Console.WriteLine("[Prefix] CheckFifteenBossesDefeatedEpoch started for " +
                              localPlayer.Character.GetType().Name);
            return localPlayer.Character is not Watcher; // skip original for Watcher
        }

        private static void Postfix(ProgressSaveManager __instance, Player localPlayer)
        {
            Console.WriteLine("[Postfix] CheckFifteenBossesDefeatedEpoch finished for " +
                              localPlayer.Character.GetType().Name);
        }
    }

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
            return localPlayer.Character is not Watcher;
        }

        private static void Postfix(ProgressSaveManager __instance, Player localPlayer, int act)
        {
            Console.WriteLine(
                $"[Postfix] ObtainCharUnlockEpoch finished for {localPlayer.Character.GetType().Name}, Act {act + 1}");
        }
    }
}