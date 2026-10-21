using HarmonyLib;
using MegaCrit.Sts2.Core.Helpers;
using Watcher.Code.Audio;

namespace Watcher.Code.Patches;

// Vanilla's own SfxCmd.Play / NAudioManager.PlayOneShot resolve our event paths natively once our
// banks are loaded (they share the base game's FmodServer Studio System instance). No interception
// needed there — this patch only exists to flush our queued RegisterBank calls once deferred init completes.
[HarmonyPatch(typeof(OneTimeInitialization), nameof(OneTimeInitialization.ExecuteDeferred))]
internal static class DeferredInitializationFmodFlushPatch
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        try
        {
            FmodStudio.OnDeferredInitializationCompleted();
        }
        catch (Exception ex)
        {
            WatcherMainFile.Logger.Warn($"[Audio] deferred FMOD flush hook failed: {ex.Message}");
        }
    }
}
