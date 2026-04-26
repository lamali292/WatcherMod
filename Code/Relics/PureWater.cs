using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Watcher.Code.Abstract;
using Watcher.Code.Cards.Token;
using Watcher.Code.Character;

namespace Watcher.Code.Relics;

[Pool(typeof(WatcherRelicPool))]
public sealed class PureWater : WatcherRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Starter;
    
    public override async Task BeforeHandDraw(
        Player player,
        PlayerChoiceContext choiceContext,
        CombatState combatState)
    {
        if (player != Owner || combatState.RoundNumber > 1) return;
        var miracle = combatState.CreateCard<Miracle>(Owner);
        await CardPileCmd.AddGeneratedCardToCombat(miracle, PileType.Hand, true);
        Flash();
    }

    public override RelicModel? GetUpgradeReplacement()
    {
        return ModelDb.Relic<HolyWater>();
    }
}
