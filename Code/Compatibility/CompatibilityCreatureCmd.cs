using System.Linq.Expressions;
using System.Reflection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Watcher.Code.Compatibility;

public static class CompatibilityCreatureCmd
{
    private delegate Task<IEnumerable<DamageResult>> DamageDel(
        PlayerChoiceContext choiceContext, Creature target, decimal amount,
        ValueProp props, CardModel cardSource, CardPlay? cardPlay);

    private static readonly DamageDel DamageD = Build();

    public static Task<IEnumerable<DamageResult>> Damage(
        PlayerChoiceContext choiceContext, Creature target, decimal amount,
        ValueProp props, CardModel cardSource, CardPlay? cardPlay)
        => DamageD(choiceContext, target, amount, props, cardSource, cardPlay);

    private static DamageDel Build()
    {
        const BindingFlags f = BindingFlags.Public | BindingFlags.Static;
        Type[] oldSig = [typeof(PlayerChoiceContext), typeof(Creature), typeof(decimal),
                         typeof(ValueProp), typeof(CardModel)];

        // New API: trailing CardPlay — exact delegate match, no expression tree needed.
        var withPlay = typeof(CreatureCmd).GetMethod("Damage", f, null, [.. oldSig, typeof(CardPlay)], null);
        if (withPlay != null)
            return withPlay.CreateDelegate<DamageDel>();

        // Old API (V107): compile a wrapper that accepts and drops cardPlay.
        var method = typeof(CreatureCmd).GetMethod("Damage", f, null, oldSig, null)
            ?? throw new MissingMethodException("CreatureCmd.Damage(ctx, Creature, decimal, ValueProp, CardModel) not found.");

        var ps = new[]
        {
            Expression.Parameter(typeof(PlayerChoiceContext), "choiceContext"),
            Expression.Parameter(typeof(Creature), "target"),
            Expression.Parameter(typeof(decimal), "amount"),
            Expression.Parameter(typeof(ValueProp), "props"),
            Expression.Parameter(typeof(CardModel), "cardSource"),
            Expression.Parameter(typeof(CardPlay), "cardPlay"), // dropped
        };
        var call = Expression.Call(method, ps[..^1].Cast<Expression>());
        return Expression.Lambda<DamageDel>(call, ps).Compile();
    }
}