using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using WatcherMod.Models.Cards;

namespace WatcherMod.Relics;

public sealed class PureWater : RelicModel
{
    public override RelicRarity Rarity => RelicRarity.Starter;


    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        var pureWater = this;
        if (side != pureWater.Owner.Creature.Side || combatState.RoundNumber > 1)
            return;

        var miracle = combatState.CreateCard<Miracle>(Owner);
        await CardPileCmd.AddGeneratedCardToCombat(miracle, PileType.Hand, true);
        pureWater.Flash();
    }
}