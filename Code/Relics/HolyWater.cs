using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using Watcher.Code.Cards.Token;
using Watcher.Code.Character;
using Watcher.Code.Extensions;

namespace Watcher.Code.Relics;

[Pool(typeof(WatcherRelicPool))]
public sealed class PureWater : CustomRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    public override string PackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".RelicImagePath();


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