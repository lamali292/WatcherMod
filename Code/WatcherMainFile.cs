using System.Reflection;
using Godot;
using Godot.Bridge;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using Watcher.Code.Compatibility;
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

        var patcher = ModPatcher.Create(ModId, Logger);
        patcher.Add(typeof(StanceModifyDamageMultiplicativePatch))
            .Add(typeof(PaelsEyeSourceTrackingPatches))
            .Add(typeof(ColorfulPhilosophersPatch))
            .Add(typeof(WatcherAnimationPatch))
            .Add(typeof(WatcherDeathAnimPatch))
            .Add(typeof(NEnergyCounterReadyPatch))
            .Add(typeof(ModelDbInitIdsPatch))
            .Add(typeof(FakeMerchantAnimationPatch));

        patcher.Add(AccessTools.TypeByName("MegaCrit.Sts2.Core.Entities.Cards.CardLocation") != null
            ? typeof(ModifyCardPlayResultLocationNewPatch)
            : typeof(ModifyCardPlayResultLocationOldPatch));

        patcher.PatchAll();
    }
}

