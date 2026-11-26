using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Encounters;
using Watcher.Code.Cards.Basic;
using Watcher.Code.Cards.Rare;
using Watcher.Code.Core;
using Watcher.Code.Powers;
using Watcher.Code.Stances;

namespace Watcher.Code.Tests;

public class WatcherTests
{
    [CardTest(typeof(Character.Watcher))]
    public IEnumerable<CardTestCase> PlayWatcherCards(CharacterModel character) => AllCardsTest.PlayAllCards(character);

    // AfterDeath hook (signature differs between game versions, see AfterDeathSourceCompatPatch): dying must end the stance.
    [CardTest(typeof(Character.Watcher))]
    public async Task PlayerDeathEndsStance(TestContext ctx)
    {
        await WatcherModel.SetStance<WrathStance>(new BlockingPlayerChoiceContext(), ctx.Player, null);
        Assert.IsTrue(WatcherModel.IsInStance<WrathStance>(ctx.Player), "Precondition: player should be in Wrath.");

        await CreatureCmd.Kill(ctx.Player.Creature, true);

        Assert.IsTrue(WatcherModel.IsInStance<NoStance>(ctx.Player), "Stance should be reset when the player dies.");
    }

    // AfterDeath hook: a killing blow on a creature carrying BlockReturnPower returns its block once. AfterAttack
    // doesn't run once the owner is dead, so the death hook is the only source of that block; a doubled hook
    // (e.g. two mods forwarding the same compat shim) would show up as extra block.
    // Multi-enemy encounter: killing the only enemy would end the combat and wipe the block being measured.
    [CardTest(typeof(Character.Watcher), typeof(CorpseSlugsNormal))]
    public async Task BlockReturnGivesBlockOnKill(TestContext ctx)
    {
        var enemy = ctx.Combat.HittableEnemies.First();
        await PowerCmd.Apply<BlockReturnPower>(new BlockingPlayerChoiceContext(), enemy, 5, ctx.Player.Creature, null);
        await CreatureCmd.SetCurrentHp(enemy, enemy.MaxHp);

        var before = ctx.Player.Creature.Block;
        await ctx.PlayCard(await ctx.AddCardToHand<StrikeWatcher>(), enemy);
        var nonLethal = ctx.Player.Creature.Block - before;
        Assert.IsTrue(nonLethal > 0, "Precondition: a non-lethal hit should return block.");
        Assert.IsTrue(enemy.IsAlive, "Precondition: the first hit must not kill.");

        await CreatureCmd.SetCurrentHp(enemy, 1);
        before = ctx.Player.Creature.Block;
        await ctx.PlayCard(await ctx.AddCardToHand<StrikeWatcher>(), enemy);
        var lethal = ctx.Player.Creature.Block - before;

        Assert.IsTrue(!enemy.IsAlive, "Precondition: the second hit should kill.");
        Assert.IsTrue(lethal == nonLethal, $"Killing blow should return the power's block exactly once (non-lethal {nonLethal}, lethal {lethal}).");
    }

    // Regression: the Fatal upgrade must still happen when the killing blow ends the combat. CardCmd.Upgrade skips
    // once combat is ending, so LessonLearned upgrades by hand. Lone enemy => this kill ends the combat.
    [CardTest(typeof(Character.Watcher))]
    public async Task LessonLearnedUpgradesWhenKillEndsCombat(TestContext ctx)
    {
        var enemies = ctx.Combat.HittableEnemies.ToList();
        foreach (var other in enemies.Skip(1)) await CreatureCmd.Kill(other, true);
        var enemy = enemies[0];
        await CreatureCmd.SetCurrentHp(enemy, 1);

        var deck = PileType.Deck.GetPile(ctx.Player).Cards;
        var upgradedBefore = deck.Count(c => c.IsUpgraded);
        Assert.IsTrue(deck.Any(c => c.IsUpgradable), $"Precondition: needs an upgradable deck card (deck has {deck.Count()} cards).");
        await ctx.PlayCard(await ctx.AddCardToHand<LessonLearned>(), enemy);

        Assert.IsTrue(!enemy.IsAlive, "Precondition: the hit should kill the last enemy.");
        Assert.IsTrue(deck.Count(c => c.IsUpgraded) == upgradedBefore + 1,
            $"Killing the last enemy should upgrade one deck card ({upgradedBefore} upgraded before, {deck.Count(c => c.IsUpgraded)} after).");
    }
}
