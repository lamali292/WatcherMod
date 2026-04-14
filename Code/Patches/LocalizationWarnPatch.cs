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

        var tableName = Traverse.Create(__instance).Field("_name").GetValue<string>();
        GD.PushWarning($"[LocTable] GetLocString: Key '{key}' not found in table '{tableName}'");
        __result = new LocString(tableName, key);
        return false;
    }

    [HarmonyPatch(nameof(LocTable.GetRawText))]
    [HarmonyPrefix]
    public static bool Prefix(LocTable __instance, string key, ref string __result)
    {
        if (__instance.HasEntry(key))
            return true;

        var tableName = Traverse.Create(__instance).Field("_name").GetValue<string>();
        GD.PushWarning($"[LocTable] GetRawText: Key '{key}' not found in table '{tableName}'");
        __result = key;
        return false;
    }
}