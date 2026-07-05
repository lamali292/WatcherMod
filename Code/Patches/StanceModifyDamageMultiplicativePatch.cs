using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Watcher.Code.Stances;

namespace Watcher.Code.Patches;

[HarmonyPatch]
internal static class StanceModifyDamageMultiplicativePatch
{
    private static MethodBase TargetMethod()
    {
        const BindingFlags f = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        Type[] oldSig = [typeof(Creature), typeof(decimal), typeof(ValueProp),
            typeof(Creature), typeof(CardModel)];

        return typeof(AbstractModel).GetMethod("ModifyDamageMultiplicative", f, null,
                   [.. oldSig, typeof(CardPlay)], null)
               ?? typeof(AbstractModel).GetMethod("ModifyDamageMultiplicative", f, null, oldSig, null)
               ?? throw new MissingMethodException("Stance ModifyDamageMultiplicative not found in any known signature.");
    }

    [HarmonyPostfix]
    private static void Postfix(object __instance, object[] __args, ref decimal __result)
    {
        if (__instance is not WatcherStanceModel stance) return;
        __result *= stance.WatcherModifyDamageMultiplicative(
            (Creature?)__args[0], (decimal)__args[1], (ValueProp)__args[2],
            (Creature?)__args[3], (CardModel?)__args[4],
            __args.Length > 5 ? (CardPlay?)__args[5] : null);
    }
}