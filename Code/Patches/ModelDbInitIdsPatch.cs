using HarmonyLib;
using MegaCrit.Sts2.Core.Models;

namespace Watcher.Code.Patches;

[HarmonyPatch(typeof(ModelDb), "InitIds")]
internal static partial class ModelDbInitIdsPatch
{
    [HarmonyPostfix]
    private static void LogRegisteredCounts()
    {
        var modAssembly = typeof(WatcherMainFile).Assembly;
        var characters = ModelDb.AllCharacters
            .Where(c => c.GetType().Assembly == modAssembly)
            .ToList();

        foreach (var character in characters.OrderBy(c => c.Id.Entry))
        {
            var charName = character.GetType().Name;
            var cards = ModelDb.AllCards.Count(c => c.Pool == character.CardPool);
            var relics = ModelDb.AllRelics.Count(r => r.Pool == character.RelicPool);
            var potions = ModelDb.AllPotions.Count(p => p.Pool == character.PotionPool);
            WatcherMainFile.Logger.Info($"{charName}: {cards} cards, {relics} relics, {potions} potions");
        }

        var powers = ModelDb.AllPowers.Count(p => p.GetType().Assembly == modAssembly);
        WatcherMainFile.Logger.Info($"Powers: {powers}");
    }
}