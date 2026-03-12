using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Unlocks;

namespace WatcherMod.Patches;

[HarmonyPatch(typeof(Player), nameof(Player.CreateForNewRun), typeof(CharacterModel), typeof(UnlockState),
    typeof(ulong))]
public class ExamplePatch
{
    private static void Postfix(Player __result)
    {
        //var watcherPool = ModelDb.CardPool<WatcherCardPool>();
        //var latestCards = watcherPool.AllCards.TakeLast(10);

        //__result.Deck.AddInternal(ModelDb.Card<FearNoEvil>().ToMutable());
        //foreach (var card in latestCards) __result.Deck.AddInternal(card.ToMutable());
        //var customRelic = ModelDb.Relic<Melange>().ToMutable();
        //__result.AddRelicInternal(customRelic);
    }
}