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
        // Get 3 random distinct attack cards from ALL card pools
        var allAttacks = ModelDb.AllCards.Where(c => c.Type == CardType.Attack);

        var randomAttacks = CardFactory.GetDistinctForCombat(
            Owner,
            allAttacks,
            3, // Get 3 cards
            Owner.RunState.Rng.CombatCardGeneration
        ).ToList();

        if (randomAttacks.Any())
        {
            // Let player choose 1 of 3
            var chosenCard = await CardSelectCmd.FromChooseACardScreen(
                choiceContext,
                randomAttacks,
                Owner
            );

            if (chosenCard != null)
            {
                // If upgraded, make it cost 0 this turn
                if (IsUpgraded) chosenCard.SetToFreeThisTurn();

                // Add to hand
                await CardPileCmd.AddGeneratedCardToCombat(
                    chosenCard,
                    PileType.Hand,
                    true
                );
            }
        }
    }

    protected override void OnUpgrade()
    {
        // Upgrade effect: chosen card costs 0 this turn
        // (handled in OnPlay via SetToFreeThisTurn)
    }
}