using Watcher.Code.Compatibility;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Watcher.Code.Tests;

public class AllCardsTest
{
 
    public static IEnumerable<CardTestCase> PlayAllCards(CharacterModel character)
    {
        return character.CardPool.AllCards.Select(model => new CardTestCase(model.GetType().Name, async ctx =>
        {
            TestMainFile.Logger.Info($"CardTestCase : {model.Title}");
            // combat is already fresh (runner called FreshCombat); relics already granted once.
            var card = ctx.Combat.CreateCard(model, ctx.Player);
            await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, ctx.Player);

            var title = card.Title;
            var description = card.GetDescriptionForPile(PileType.Hand);
            Assert.IsTrue(!string.IsNullOrEmpty(title) && !title.Contains(card.Id.Entry),
                $"Title for {card.Id.Entry} looks unresolved: '{title}'.");
            Assert.IsTrue(!string.IsNullOrEmpty(description) && !description.Contains(card.Id.Entry),
                $"Description for {card.Id.Entry} looks unresolved: '{description}'.");

            var target = card.TargetType == TargetType.AnyEnemy
                ? ctx.Combat.HittableEnemies.FirstOrDefault()
                : null;
            if (target != null)
            {
                await CreatureCmd.SetMaxHp(target, 9999);
                await CreatureCmd.SetCurrentHp(target, 9999);
            }

            await ctx.PlayCard(card, target);

            PlayerCmd.EndTurn(ctx.Player, false);
            
            var card2 = ctx.Combat.CreateCard(model, ctx.Player);
            await CardPileCmd.AddGeneratedCardToCombat(card2, PileType.Draw, ctx.Player, CardPilePosition.Top);
            var a = await CardPileCmd.Draw(new BlockingPlayerChoiceContext(), ctx.Player);
            if (a != null) Assert.AreEqual(card2, a);

            PlayerCmd.EndTurn(ctx.Player, false);
            
            var card3 = ctx.Combat.CreateCard(model, ctx.Player);
            await CardPileCmd.AddGeneratedCardToCombat(card3, PileType.Hand, ctx.Player);
            var b = await CardCmdCompatibility.Exhaust(new BlockingPlayerChoiceContext(), card3);
            if (b.HasValue) Assert.AreEqual(card3, b.Value.cardAdded);

            PlayerCmd.EndTurn(ctx.Player, false);
            
            var card4 = ctx.Combat.CreateCard(model, ctx.Player);
            await CardPileCmd.AddGeneratedCardToCombat(card4, PileType.Hand, ctx.Player);
            await CardCmd.Discard(new BlockingPlayerChoiceContext(), card4);

            PlayerCmd.EndTurn(ctx.Player, false);
            
            var strike = ctx.Combat.CreateCard(ModelDb.Card<StrikeIronclad>(), ctx.Player);
            var defend = ctx.Combat.CreateCard(ModelDb.Card<DefendIronclad>(), ctx.Player);
            await CardPileCmd.AddGeneratedCardToCombat(strike, PileType.Hand, ctx.Player);
            await CardPileCmd.AddGeneratedCardToCombat(defend, PileType.Hand, ctx.Player);
            await ctx.PlayCard(strike, target);
            await ctx.PlayCard(defend, target);

            var card5 = ctx.Combat.CreateCard(model, ctx.Player);
            await CardPileCmd.AddGeneratedCardToCombat(card5, PileType.Hand, ctx.Player);
        }));
    }
}