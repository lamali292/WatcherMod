using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using WatcherMod.Models.Characters;

[ModInitializer("Initialize")]
public class ModEntry
{
    public static void Initialize()
    {
        var harmony = new Harmony("watchermod.patch");

        //ProgressSaveManagerCustomCharPatch.Apply(harmony);
        harmony.PatchAll();
    }
}


[HarmonyPatch(typeof(ModelDb), "AllCharacters", MethodType.Getter)]
[HarmonyPriority(Priority.First)]
public class ModelDbAllCharactersPatch
{
    private static void Postfix(ref IEnumerable<CharacterModel> __result)
    {
        // Add Watcher to the list of all characters
        var charactersList = __result.ToList();
        charactersList.Add(ModelDb.Character<Watcher>());


        __result = charactersList;

        typeof(ModelDb).GetField("_allCharacterCardPools", BindingFlags.Static | BindingFlags.NonPublic)
            ?.SetValue(null, null);
        typeof(ModelDb).GetField("_allCards", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, null);
    }
}