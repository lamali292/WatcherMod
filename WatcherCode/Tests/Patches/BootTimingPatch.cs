using HarmonyLib;
using MegaCrit.Sts2.Core.Helpers;

namespace Watcher.Code.Tests;

/// Starts the automated WATCHER_RUN_TESTS=1 run right after OneTimeInitialization.ExecuteEssential
/// ("essential initialization needed before main menu can display" - mods/loc/ModelDb), instead of
/// waiting for MainMenuButtonRegistry.MainMenuReady. Measured ~2.4s earlier on a local run (3.6s vs
/// 6.0s from mod-init) - CardTestRunner.NewCombatAsync only touches ModelDb/RunState/CombatManager,
/// never anything main-menu-specific, so there's nothing to wait for. The interactive "Unit Test"
/// button (see TestMainFile) is unaffected - it still needs the real menu to click.
[HarmonyPatch(typeof(OneTimeInitialization), nameof(OneTimeInitialization.ExecuteEssential))]
internal static class BootTimingPatch
{
    [HarmonyPostfix]
    private static void Postfix()
    {
        TestMainFile.OnEssentialReady();
    }
}
