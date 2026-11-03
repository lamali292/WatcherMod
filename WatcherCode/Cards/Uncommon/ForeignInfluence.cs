using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using Watcher.Code.Abstract;
using Watcher.Code.Character;
using Watcher.Code.Extensions;

namespace Watcher.Code.Cards.Uncommon;

[Pool(typeof(WatcherCardPool))]
public sealed class ForeignInfluence : WatcherCardModel
{
    public ForeignInfluence() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithKeywords(CardKeyword.Exhaust);
    }


    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (Owner.Creature.CombatState == null) return;

        // Attacks of any colour (own and colourless included), like STS1. Taken from the unlocked pools, so
        // restrictions from other mods apply; the game filters rarity, player count and generatability.
        var attacks = Owner.UnlockState.CharacterCardPools
            .Append(ModelDb.CardPool<ColorlessCardPool>())
            .SelectMany(pool => pool.GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint))
            .Where(c => c.Type == CardType.Attack);
        var options = CardFactory.GetDistinctForCombat(Owner, attacks, 3,
            Owner.RunState.Rng.CombatCardGeneration).ToList();
        if (options.Count == 0) return;

        // Let player choose 1 of 3
        var chosenCard = await CardSelectCmd.FromChooseACardScreen(
            choiceContext,
            options,
            Owner,
            true
        );

        if (chosenCard == null) return;

        if (IsUpgraded) chosenCard.SetToFreeThisTurn();

        await CardPileCmd.AddGeneratedCardToCombat(
            chosenCard,
            PileType.Hand,
            Owner
        );
    }
}