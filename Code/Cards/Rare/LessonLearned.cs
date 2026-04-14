using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Watcher.Code.Character;
using Watcher.Code.Extensions;
using Watcher.Code.Powers;

namespace Watcher.Code.Cards.Rare;

[Pool(typeof(WatcherCardPool))]
public sealed class LessonLearned() : CustomCardModel(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    // Base damage
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(10m, ValueProp.Move)
    ];

    public override HashSet<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<LikeWaterPower>()
    ];

    public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        var target = cardPlay.Target;
        var damageAmount = DynamicVars.Damage.BaseValue;

        // Apply normal damage
        Log.Info("Lesson learned damage");
        await DamageCmd.Attack(damageAmount)
            .FromCard(this)
            .Targeting(target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    public override Task AfterDamageGiven(
        PlayerChoiceContext choiceContext,
        Creature? target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        Log.Info("damage received");
        if (cardSource == null || cardSource.Id != Id) return Task.CompletedTask;
        Log.Info("damage received lesson learned");
        if (result.WasTargetKilled)
        {
            Log.Info("was killed");
            UpgradeRandomCard(choiceContext);
        }
        else
        {
            Log.Info("was not killed");
        }

        return Task.CompletedTask;
    }


    private void UpgradeRandomCard(PlayerChoiceContext choiceContext)
    {
        var deckCards = Owner.Deck.Cards.ToList();
        if (deckCards.Count == 0)
            return;

        var randomCard = Owner.RunState.Rng.CombatTargets.NextItem(deckCards);
        if (randomCard != null) CardCmd.Upgrade(randomCard);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m); // 10 -> 13
    }
}