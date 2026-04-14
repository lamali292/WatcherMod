using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;

namespace Watcher.Code.Commands;

public class WatcherCmd
{
    public static async Task GiveCard<T>(Player player,
        PileType pileType,
        int amount = 1,
        float animationTime = 0.6f,
        CardPreviewStyle animationStyle = CardPreviewStyle.HorizontalLayout) where T : CardModel
    {
        var cardsToGive = new List<CardModel>();
        var combatState = player.Creature.CombatState;
        if (combatState == null) return;
        for (var i = 0; i < amount; i++)
        {
            var card = combatState.CreateCard(ModelDb.Card<T>(), player);
            cardsToGive.Add(card);
        }

        var result = await CardPileCmd.AddGeneratedCardsToCombat(cardsToGive, pileType, true);
        CardCmd.PreviewCardPileAdd(result, animationTime, animationStyle);
    }
}