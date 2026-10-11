using Watcher.Code.Compatibility;
using Watcher.Code.Patches;
using Godot;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Saves;
using Logger = MegaCrit.Sts2.Core.Logging.Logger;

namespace Watcher.Code.Tests;


[ModInitializer(nameof(Initialize))]
public static class TestMainFile
{
    public const string ModId = "WatcherTest"; //At the moment, this is used only for the Logger and harmony names.

    /// Set to "1" to run the tests automatically once the main menu is up, then quit the game.
    /// Exit code is 0 when everything passed, 1 otherwise. See build/test.ps1.
    private const string EnvRunTests = "WATCHER_RUN_TESTS";

    /// Optional substring filter on "Type.Method" (case-insensitive).
    private const string EnvFilter = "WATCHER_TEST_FILTER";

    /// Where the JSON result is written. Defaults to user://watcher_tests.json.
    private const string EnvOutput = "WATCHER_TEST_OUTPUT";

    private static bool _autoRunStarted;

    public static Logger Logger { get; } =
        new(ModId, LogType.Generic);

    public static void Initialize()
    {
        if (System.Environment.GetEnvironmentVariable(EnvRunTests) != "1") return;
        ModPatcher.Create(ModId, Logger)
            .Add(typeof(BootTimingPatch))
            .Add(typeof(ScreenShakeTestModePatch))
            .Add(typeof(FtueTestModePatch))
            .PatchAll();

        /*
        MainMenuButtonRegistry.Register(new MainMenuButtonRegistry.Entry
        {
            Label = "Unit Test",
            IsVisible = () => true,
            SubmenuType = null,
            CreateSubmenu = null,
            OnPress = _ => TaskHelper.RunSafely(RunTests(System.Environment.GetEnvironmentVariable(EnvFilter)))
        });
        */
    }

    /// Fired via BootTimingPatch right after OneTimeInitialization.ExecuteEssential - well before
    /// the main menu itself is ready. Only starts the automated WATCHER_RUN_TESTS=1 run; the
    /// interactive "Unit Test" button is unaffected (it needs the real menu to click anyway).
    internal static void OnEssentialReady()
    {
        if (System.Environment.GetEnvironmentVariable(EnvRunTests) != "1") return;
        if (_autoRunStarted) return;
        _autoRunStarted = true;
        // Let the current call stack (still inside NGame.GameStartup) unwind before we start
        // creating combat state and card nodes.
        Callable.From(() => { TaskHelper.RunSafely(AutoRunAsync()); }).CallDeferred();
    }

    private static async Task AutoRunAsync()
    {
        var exitCode = 1;
        try
        {
            await WaitForSaveManagerReady();
            var result = await RunTests(System.Environment.GetEnvironmentVariable(EnvFilter));
            var output = System.Environment.GetEnvironmentVariable(EnvOutput);
            if (string.IsNullOrEmpty(output))
                output = ProjectSettings.GlobalizePath("user://watcher_tests.json");
            result.WriteJson(output);
            Logger.Info($"Test results written to {output}");
            exitCode = result.Success ? 0 : 1;
        }
        catch (Exception e)
        {
            Logger.Error($"Test run crashed: {e}");
        }
        finally
        {
            var tree = (SceneTree)Engine.GetMainLoop();
            tree.Quit(exitCode);
        }
    }

    /// NGame.GameStartup only finishes SaveManager.Instance.InitPrefsData() a few steps after
    /// OneTimeInitialization.ExecuteEssential() returns (a profile-id/cloud-sync stretch in
    /// between, which can yield to the engine's frame loop). BootTimingPatch fires right on
    /// ExecuteEssential, so without this wait, some card effects that read
    /// SaveManager.Instance.PrefsSave (e.g. TalkCmd.Play's FastMode check) would intermittently
    /// NullReferenceException depending on whether our deferred test run got scheduled before or
    /// after GameStartup's continuation reached InitPrefsData - see PrefsSaveManager.IsLoaded's
    /// doc comment: Prefs is null until LoadPrefs runs, despite the non-nullable annotation.
    private static async Task WaitForSaveManagerReady()
    {
        var tree = (SceneTree)Engine.GetMainLoop();
        for (var i = 0; i < 300 && !SaveManager.Instance.ArePrefsLoaded(); i++)
            await tree.ToSignal(tree, SceneTree.SignalName.ProcessFrame);
    }

    private static Task<TestRunResult> RunTests(string? filter)
    {
        var cts = new CancellationTokenSource(TimeSpan.FromMinutes(10));
        var harness = new CardTestRunner();
        return harness.RunAllTestsAsync(SeedHelper.GetRandomSeed(), cts.Token, filter);
    }
}
