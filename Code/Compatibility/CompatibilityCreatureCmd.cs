using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Watcher.Code.Compatibility;

public static class CompatibilityCreatureCmd
{

    public static Task<IEnumerable<DamageResult>> Damage(
        PlayerChoiceContext choiceContext,
        Creature target,
        decimal amount,
        ValueProp props,
        CardModel cardSource,
        CardPlay? cardPlay)
    {
        
#if V107
         return CreatureCmd.Damage(choiceContext, target, amount, props, cardSource);
#else
        return CreatureCmd.Damage(choiceContext, target, amount, props, cardSource, cardPlay);
#endif
    }

}