using BaseLib.Utils;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Relics;
using Watcher.Code.Abstract;
using Watcher.Code.Character;
using Watcher.Code.Powers;

namespace Watcher.Code.Cards.Rare;

[Pool(typeof(WatcherCardPool))]
public sealed class Vault : WatcherCardModel
{
    public Vault() : base(3, CardType.Skill, CardRarity.Rare, TargetType.None)
    {
        WithKeywords(CardKeyword.Exhaust);
        WithCostUpgradeBy(-1);
        WithPower<VaultPower>(1, false);
    }
    
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<VaultPower>(ctx, this);
        PlayerCmd.EndTurn(Owner, false);
    }
}


[HarmonyPatch(typeof(PaelsEye))]
public static class PaelsEyeSourceTracking
{
    // Did PaelsEye itself vote "yes" for this owner on the current side-switch?
    private static readonly HashSet<PaelsEye> SelfTriggered = [];

    [HarmonyPostfix, HarmonyPatch(nameof(PaelsEye.ShouldTakeExtraTurn))]
    public static void RecordVote(PaelsEye __instance, Player player, bool __result)
    {
        if (__result && player == __instance.Owner) SelfTriggered.Add(__instance);
        else                                        SelfTriggered.Remove(__instance);
    }

    [HarmonyPrefix, HarmonyPatch(nameof(PaelsEye.AfterTakingExtraTurn))]
    public static bool GateConsumption(PaelsEye __instance, Player player, ref Task __result)
    {
        if (player != __instance.Owner) return true;      
        if (SelfTriggered.Remove(__instance)) return true; 
        __result = Task.CompletedTask;                
        return false;                               
    }

    [HarmonyPostfix, HarmonyPatch(nameof(PaelsEye.AfterCombatEnd))]
    public static void Cleanup(PaelsEye __instance) => SelfTriggered.Remove(__instance);
}