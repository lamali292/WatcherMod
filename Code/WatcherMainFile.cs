using System.Reflection;
using Godot;
using Godot.Bridge;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using Watcher.Code.Events;
using Watcher.Code.Patches;
using Logger = MegaCrit.Sts2.Core.Logging.Logger;

namespace Watcher.Code;

[ModInitializer(nameof(Initialize))]
public partial class WatcherMainFile : Node
{
    public const string ModId = "Watcher"; //At the moment, this is used only for the Logger and harmony names.

    public static Logger Logger { get; } =
        new(ModId, LogType.Generic);

    public static void Initialize()
    {
        WatcherSubscriber.Subscribe();
        var assembly = Assembly.GetExecutingAssembly();
        ScriptManagerBridge.LookupScriptsInAssembly(assembly);
        Harmony harmony = new(ModId);
        
        ApplyPatch(harmony, typeof(StanceModifyDamageMultiplicativePatch));
        ApplyPatch(harmony, typeof(PaelsEyeSourceTrackingPatches));
        ApplyPatch(harmony, typeof(ColorfulPhilosophersPatch));
        ApplyPatch(harmony, typeof(WatcherAnimationPatch));
        ApplyPatch(harmony, typeof(WatcherDeathAnimPatch));
        ApplyPatch(harmony, typeof(NEnergyCounterReadyPatch));
        ApplyPatch(harmony, typeof(ModelDbInitIdsPatch));
    }
    
    private static void ApplyPatch(Harmony harmony, Type patchClass)
    {
        try
        {
            var patched = harmony.CreateClassProcessor(patchClass).Patch();
            if (patched == null || patched.Count == 0)
                Logger.Error($"{patchClass.Name}: applied but patched ZERO methods (TargetMethod returned null?).");
            else
                Logger.Info($"{patchClass.Name}: OK ({patched.Count} method(s)).");
        }
        catch (Exception ex)
        {
            Logger.Error($"{patchClass.Name}: FAILED to apply.\n{ex}");
        }
    }
}

