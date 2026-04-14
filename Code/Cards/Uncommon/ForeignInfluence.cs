using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Watcher.Code.Character;
using Watcher.Code.Extensions;

namespace Watcher.Code.Cards.Uncommon;

[Pool(typeof(WatcherCardPool))]
public sealed class ForeignInfluence() : CustomCardModel(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override HashSet<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (Owner.Creature.CombatState == null) return;
        var rng = Owner.RunState.Rng.CombatCardGeneration;

        // Build 3 distinct weighted-rarity attack cards from ALL pools
        var attacksByRarity = CardFactory.FilterForCombat(ModelDb.AllCards)
            .Where(c => c.Type == CardType.Attack)
            .GroupBy(c => c.Rarity)
            .ToDictionary(g => g.Key, g => g.ToList());

        var weightedAttacks = new List<CardModel>();
        var seen = new HashSet<string>();

        while (weightedAttacks.Count < 3)
        {
            var roll = rng.NextInt(100);
            var rarity = roll < 55 ? CardRarity.Common
                : roll < 85 ? CardRarity.Uncommon
                : CardRarity.Rare;

            if (!attacksByRarity.TryGetValue(rarity, out var pool)) continue;

            var candidate = rng.NextItem(pool);
            if (candidate == null || !seen.Add(candidate.Id.Entry)) continue;

            weightedAttacks.Add(Owner.Creature.CombatState.CreateCard(candidate, Owner));
        }

        // Let player choose 1 of 3
        var chosenCard = await CardSelectCmd.FromChooseACardScreen(
            choiceContext,
            weightedAttacks,
            Owner,
            true
        );

        if (chosenCard != null)
        {
            if (IsUpgraded) chosenCard.SetToFreeThisTurn();

            await CardPileCmd.AddGeneratedCardToCombat(
                chosenCard,
                PileType.Hand,
                true
            );
        }
    }

    protected override void OnUpgrade()
    {
        // Upgrade effect: chosen card costs 0 this turn
        // (handled in OnPlay via SetToFreeThisTurn)
    }
}