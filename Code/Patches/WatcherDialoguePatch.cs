using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Ancients;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;

namespace Watcher.Code.Patches;

internal static class WatcherDialogueHelper
{
    public static void AddWatcherDialogues(AncientDialogueSet dialogueSet, List<AncientDialogue> dialogues)
    {
        var watcherKey = ModelDb.Character<Character.Watcher>().Id.Entry;
        dialogueSet.CharacterDialogues.TryAdd(watcherKey, dialogues);
    }
}

[HarmonyPatch(typeof(TheArchitect), "DefineDialogues")]
public static class ArchitectDialoguePatch
{
    private static void Postfix(ref AncientDialogueSet __result)
    {
        WatcherDialogueHelper.AddWatcherDialogues(__result,
        [
            new AncientDialogue("", "")
            {
                VisitIndex = 0, EndAttackers = ArchitectAttackers.Both
            },
            new AncientDialogue("", "", "", "")
            {
                VisitIndex = 1, EndAttackers = ArchitectAttackers.Both, IsRepeating = true
            }
        ]);
    }
}