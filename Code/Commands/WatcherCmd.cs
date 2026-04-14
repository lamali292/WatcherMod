using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;

namespace Watcher.Code.Commands;

public class WatcherCmd
{
    public static async Task<CardModel?> GiveCard<T>(Player player,
        PileType pileType,
        CardPilePosition pos = CardPilePosition.Bottom,
        float animationTime = 0.6f,
        CardPreviewStyle animationStyle = CardPreviewStyle.HorizontalLayout,
        bool upgraded = false) where T : CardModel
    {
        var combatState = player.Creature.CombatState;
        if (combatState == null) return null;
        var card = combatState.CreateCard(ModelDb.Card<T>(), player);
        if (upgraded)
            CardCmd.Upgrade(card);
        var result = await CardPileCmd.AddGeneratedCardToCombat(card, pileType, true, pos);
        CardCmd.PreviewCardPileAdd(result, animationTime, animationStyle);
        return card;
    }
    
    public static async Task GiveCards<T>(Player player,
        int amount,
        PileType pileType,
        CardPilePosition pos = CardPilePosition.Bottom,
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

        var result = await CardPileCmd.AddGeneratedCardsToCombat(cardsToGive, pileType, true, pos);
        CardCmd.PreviewCardPileAdd(result, animationTime, animationStyle);
    }
}