using System.Reflection;
using BaseLib.Utils;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using Watcher.Code.Compatibility;

namespace Watcher.Code.Patches;

/// <summary>
///     New game version only. Compiled against the OLD assembly, so CardLocation must never
///     appear in typed code — accessed via reflection/Traverse only.
///     Must only be added when CardLocation exists at runtime (see DownfallPatchManager).
/// </summary>
[HarmonyPatch]
public static class ModifyCardPlayResultLocationNewPatch
{
    private static readonly Type? CardLocationType =
        AccessTools.TypeByName("MegaCrit.Sts2.Core.Entities.Cards.CardLocation");

    static MethodBase TargetMethod() =>
        AccessTools.Method(typeof(Hook), "ModifyCardPlayResultLocation");

    // __result as object: Harmony boxes the CardLocation struct for us
    static void Postfix(
        ICombatState combatState,
        CardModel card,
        bool isAutoPlay,
        ResourceInfo resources,
        ref object __result,
        ref IEnumerable<AbstractModel> modifiers)
    {
        var tr = Traverse.Create(__result);
        var player   = tr.Field("player").GetValue<Player>();
        var pileType = tr.Field("pileType").GetValue<PileType>();
        var position = tr.Field("position").GetValue<CardPilePosition>();

        var result = HookUtils.Modify<IModifyCardPlayResultLocation, CardLocationCompatiblity>(
            combatState,
            new CardLocationCompatiblity(player, pileType, position),
            (m, loc) => m.ModifyCardPlayResultLocationCompability(card, isAutoPlay, resources, loc),
            out var compatModifiers);

        __result = Activator.CreateInstance(CardLocationType!,
            result.Player, result.PileType, result.Position)!;

        var added = compatModifiers.OfType<AbstractModel>().ToList();
        if (added.Count > 0)
            modifiers = modifiers.Concat(added).ToList();
    }
}


/// <summary>
///     Old game version only: dispatches <see cref="IModifyCardPlayResultLocation" /> compat listeners
///     after the vanilla <c>Hook.ModifyCardPlayResultPileTypeAndPosition</c> loop.
///     The old engine has no Player in card locations, so the compat struct carries
///     <c>Player = null</c> and any player redirection returned by listeners is dropped.
///     Must only be added when <c>CardLocation</c> does NOT exist (see DownfallPatchManager).
/// </summary>
[HarmonyPatch]
public static class ModifyCardPlayResultLocationOldPatch
{
    static MethodBase TargetMethod() =>
        AccessTools.Method(typeof(Hook), "ModifyCardPlayResultPileTypeAndPosition");

    static void Postfix(
        ICombatState combatState,
        CardModel card,
        bool isAutoPlay,
        ResourceInfo resources,
        ref (PileType, CardPilePosition) __result,
        ref IEnumerable<AbstractModel> modifiers)
    {
        var result = HookUtils.Modify<IModifyCardPlayResultLocation, CardLocationCompatiblity>(
            combatState,
            new CardLocationCompatiblity(card.Owner, __result.Item1, __result.Item2),
            (m, loc) => m.ModifyCardPlayResultLocationCompability(card, isAutoPlay, resources, loc),
            out var compatModifiers);

        __result = (result.PileType, result.Position);

        var added = compatModifiers.OfType<AbstractModel>().ToList();
        if (added.Count > 0)
            modifiers = modifiers.Concat(added).ToList();
    }
}