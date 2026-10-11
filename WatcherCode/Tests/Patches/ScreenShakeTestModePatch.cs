using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.TestSupport;

namespace Watcher.Code.Tests;

/// The [CardTest] harness (CardTestRunner.NewCombatAsync) never instantiates a real NCombatRoom,
/// so NGame's screenshake target is never set - every damage instance during a test otherwise
/// logs "Missing screenShake target!" with a full stack trace. Same category as the existing
/// TestMode.IsOn checks that skip SFX/VFX during tests (see HermitSfx, HexaghostVisualsBridge,
/// etc.) - screenshake is purely visual and has nothing to attach to here regardless.
[HarmonyPatch(typeof(NGame))]
internal static class ScreenShakeTestModePatch
{
    [HarmonyPatch(nameof(NGame.ScreenShake))]
    [HarmonyPrefix]
    private static bool SkipShake() => TestMode.IsOff;

    [HarmonyPatch(nameof(NGame.ScreenRumble))]
    [HarmonyPrefix]
    private static bool SkipRumble() => TestMode.IsOff;

    [HarmonyPatch(nameof(NGame.ScreenShakeTrauma))]
    [HarmonyPrefix]
    private static bool SkipTrauma() => TestMode.IsOff;
}
