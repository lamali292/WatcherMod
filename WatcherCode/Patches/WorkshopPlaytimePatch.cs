using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;
using MegaCrit.Sts2.Core.Platform.Steam;
using Steamworks;

namespace Watcher.Code.Patches;

/// <summary>
///     Reports playtime to Steam for the Watcher Workshop item, so it shows up in the item's playtime
///     stats. Only this item's id is passed: <see cref="SteamUGC.StartPlaytimeTracking" /> is per-item, so
///     it never affects tracking other mods started for their own items. Steam stops tracking by itself
///     when the game exits, so there is deliberately no stop call (<c>StopPlaytimeTrackingForAllItems</c>
///     would also stop other mods' items). Steam is initialized after mods load, hence the main menu hook.
/// </summary>
[HarmonyPatch(typeof(NMainMenu), nameof(NMainMenu._Ready))]
public static class WorkshopPlaytimePatch
{
    private const ulong WatcherWorkshopId = 3747492505;

    private static bool _started;

    [HarmonyPostfix]
    private static void Postfix()
    {
        if (_started || !SteamInitializer.Initialized) return;
        _started = true;
        // A throwing postfix would break the main menu's _Ready.
        try
        {
            SteamUGC.StartPlaytimeTracking([new PublishedFileId_t(WatcherWorkshopId)], 1);
        }
        catch (Exception ex)
        {
            WatcherMainFile.Logger.Warn($"[Steam] playtime tracking failed: {ex.Message}");
        }
    }
}
