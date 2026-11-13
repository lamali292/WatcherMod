using System.Collections.Concurrent;
using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Watcher.Code.Compatibility;

/// <summary>
///     Newer builds (0.113+) pass the killing card to <c>AbstractModel.AfterDeath(ctx, creature, CardModel? source,
///     wasRemovalPrevented, deathAnimLength)</c>; older ones have no <c>source</c> parameter. Models in this mod
///     override the newer signature only. On an older game the hook dispatcher calls the old virtual instead, which
///     they don't override - so this patch (active only where that overload exists) forwards the call to the model's
///     newer-signature method with <c>source = null</c>.
/// </summary>
[HarmonyPatch]
internal static class AfterDeathSourceCompatPatch
{
    private static readonly Type[] OldSignature =
        [typeof(PlayerChoiceContext), typeof(Creature), typeof(bool), typeof(float)];

    private static readonly Type[] NewSignature =
        [typeof(PlayerChoiceContext), typeof(Creature), typeof(CardModel), typeof(bool), typeof(float)];

    private static readonly ConcurrentDictionary<Type, MethodInfo?> NewOverrides = new();

    private static bool Prepare() => TargetMethod() != null;

    private static MethodBase? TargetMethod() => AccessTools.Method(typeof(AbstractModel), "AfterDeath", OldSignature);

    [HarmonyPostfix]
    private static void Postfix(AbstractModel __instance, PlayerChoiceContext choiceContext, Creature creature,
        bool wasRemovalPrevented, float deathAnimLength, ref Task __result)
    {
        // Only this mod's own models: other mods ship the same shim for theirs, and forwarding for every model
        // would make each of them fire twice when both mods are loaded.
        var forwarded = NewOverrides.GetOrAdd(__instance.GetType(), t =>
            t.Assembly != typeof(AfterDeathSourceCompatPatch).Assembly ? null : t.GetMethod("AfterDeath", BindingFlags.Public | BindingFlags.Instance, null, NewSignature, null));
        if (forwarded == null) return;

        var ours = forwarded.Invoke(__instance, [choiceContext, creature, null, wasRemovalPrevented, deathAnimLength]) as Task;
        if (ours != null) __result = Chain(__result, ours);
    }

    private static async Task Chain(Task first, Task second)
    {
        await first;
        await second;
    }
}
