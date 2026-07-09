using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Watcher.Code.Stances;

namespace Watcher.Code.Patches;

public interface IModifyDamageAdditive
{
    decimal ModifyDamageAdditiveCompability(Creature? target, decimal amount,
        ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay) => 0;
}

public interface IModifyDamageMultiplicative
{
    decimal ModifyDamageMultiplicativeCompability(Creature? target, decimal amount,
        ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay) => 1;
}

[HarmonyPatch]
internal static class StanceModifyDamageMultiplicativePatch
{
    private static MethodBase TargetMethod()
    {
        var hook = AccessTools.TypeByName("MegaCrit.Sts2.Core.Hooks.Hook")
                   ?? throw new MissingMethodException("Hook not found");
        return AccessTools.Method(hook, "ModifyDamageInternal")
               ?? throw new MissingMethodException("ModifyDamageInternal not found");
    }

    private static decimal AdditiveBridge(AbstractModel listener, decimal vanillaNum,
        Creature target, decimal amount, ValueProp props, Creature dealer,
        CardModel cardSource, CardPlay? cardPlay)
    {
        if (listener is IModifyDamageAdditive m)
            return vanillaNum + m.ModifyDamageAdditiveCompability(target, amount, props, dealer, cardSource, cardPlay);
        return vanillaNum;
    }

    private static decimal MultiplicativeBridge(AbstractModel listener, decimal vanillaNum,
        Creature target, decimal amount, ValueProp props, Creature dealer,
        CardModel cardSource, CardPlay? cardPlay)
    {
        if (listener is IModifyDamageMultiplicative m)
            return vanillaNum * m.ModifyDamageMultiplicativeCompability(target, amount, props, dealer, cardSource, cardPlay);
        return vanillaNum;
    }

    private static IEnumerable<CodeInstruction> Transpiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var code = new List<CodeInstruction>(instructions);
        var hasCardPlay = original.GetParameters().Any(p => p.ParameterType == typeof(CardPlay));

        var addMethod = AccessTools.Method(typeof(AbstractModel), "ModifyDamageAdditive");
        var mulMethod = AccessTools.Method(typeof(AbstractModel), "ModifyDamageMultiplicative");
        var addBridge = AccessTools.Method(typeof(StanceModifyDamageMultiplicativePatch), nameof(AdditiveBridge));
        var mulBridge = AccessTools.Method(typeof(StanceModifyDamageMultiplicativePatch), nameof(MultiplicativeBridge));
        
        for (var i = 0; i < code.Count; i++)
        {
            var isAdd = code[i].Calls(addMethod);
            var isMul = code[i].Calls(mulMethod);
            if (!isAdd && !isMul) continue;
            
            var storeIndex = i + 1;
            if (storeIndex >= code.Count || code[storeIndex].opcode != OpCodes.Stloc_S) continue;
            var numLocal = code[storeIndex].operand;
            
            var listenerLoad = FindListenerLoadBackwards(code, i);
            if (listenerLoad == null) continue;

            var injected = new List<CodeInstruction>
            {
                listenerLoad.Clone(),                                  
                new(OpCodes.Ldloc_S, numLocal),       
                new(OpCodes.Ldarg_2),                  
                new(OpCodes.Ldloc_0),                
                new (OpCodes.Ldarg_S, (byte)5),      
                new (OpCodes.Ldarg_3),                  
                new (OpCodes.Ldarg_S, (byte)6),       
                hasCardPlay
                    ? new CodeInstruction(OpCodes.Ldarg_S, (byte)7)    
                    : new CodeInstruction(OpCodes.Ldnull),           
                new (OpCodes.Call, isAdd ? addBridge : mulBridge),
                new (OpCodes.Stloc_S, numLocal),       
            };

            code.InsertRange(storeIndex + 1, injected);
            i = storeIndex + injected.Count; 
        }

        return code;
    }
    
    private static CodeInstruction? FindListenerLoadBackwards(List<CodeInstruction> code, int callIndex)
    {
        for (var j = callIndex - 1; j >= 0 && j > callIndex - 10; j--)
        {
            if (code[j].opcode == OpCodes.Ldarg_2) 
                return code[j - 1];              
        }
        return null;
    }
}