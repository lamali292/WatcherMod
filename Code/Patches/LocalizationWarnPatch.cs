using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;

namespace Watcher.Code.Patches;

[HarmonyPatch(typeof(LocTable))]
public static class LocalizationWarnPatch
{
    [HarmonyPatch(nameof(LocTable.GetLocString))]
    [HarmonyPrefix]
    public static bool Prefix(LocTable __instance, string key, ref LocString __result)
    {
        if (__instance.HasEntry(key))
            return true;

        GD.PushWarning($"[LocTable] GetLocString: Key not found: '{key}' — using key as placeholder");
        __result = new LocString(key, key);
        return false;
    }

    [HarmonyPatch(nameof(LocTable.GetRawText))]
    [HarmonyPrefix]
    public static bool Prefix(LocTable __instance, string key, ref string __result)
    {
        if (__instance.HasEntry(key))
            return true;

        GD.PushWarning($"[LocTable] GetRawText: Key not found: '{key}' — using key as placeholder");
        __result = key;
        return false;
    }
}