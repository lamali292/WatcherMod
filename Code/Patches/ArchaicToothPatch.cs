using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using Watcher.Code.Cards.Ancient;
using Watcher.Code.Cards.Basic;

namespace Watcher.Code.Patches;

[HarmonyPatch(typeof(ArchaicTooth), "TranscendenceUpgrades", MethodType.Getter)]
public static class ArchaicToothPatch
{
    [HarmonyPostfix]
    private static void AddWatcherTranscendence(ref Dictionary<ModelId, CardModel> __result)
    {
        __result[ModelDb.Card<Eruption>().Id] = ModelDb.Card<AncientCard2>();
    }
}