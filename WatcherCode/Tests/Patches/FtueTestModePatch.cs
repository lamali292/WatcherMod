using HarmonyLib;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.TestSupport;

namespace Watcher.Code.Tests;

/// SaveManager.SeenPopup already returns true unconditionally under TestMode.IsOn, but
/// SaveManager.SeenFtue has no such check - so the very first ShuffleIfNecessary of a whole test
/// run (or any other FTUE-gated code path) tries to actually create and show the FTUE popup
/// (e.g. NShuffleFtue), which either never gets confirmed (the test just hangs waiting for
/// WaitForPlayerToConfirm) or NullReferenceExceptions on some UI dependency headless mode never
/// set up. Whether this fires is a race against everything else the test suite happens to run
/// first - the same FTUE key only needs to trip once per whole run - so it manifested as a flaky
/// failure (e.g. AllCardsTest.PlayHermitCards:Roulette via CardPileCmd.ShuffleFtueCheck) instead of
/// a consistent one. Same category as ScreenShakeTestModePatch: purely presentational, nothing to
/// attach a popup to during a headless test run regardless.
[HarmonyPatch(typeof(SaveManager), nameof(SaveManager.SeenFtue))]
internal static class FtueTestModePatch
{
    [HarmonyPrefix]
    private static bool Prefix(ref bool __result)
    {
        if (TestMode.IsOff) return true;
        __result = true;
        return false;
    }
}
