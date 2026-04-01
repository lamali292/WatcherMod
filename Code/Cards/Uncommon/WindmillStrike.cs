using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Watcher.Code.Cards.CardModels;
using Watcher.Code.Character;
using Watcher.Code.Extensions;

namespace Watcher.Code.Cards.Uncommon;

[Pool(typeof(WatcherCardPool))]
public sealed class WindmillStrike() : WatcherCardModel(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    private const string _retainedIncreaseKey = "RetainIncrease";

    private decimal _extraDamageFromRetains;

    private decimal ExtraDamageFromRetains
    {
        get => _extraDamageFromRetains;
        set
        {
            AssertMutable();
            _extraDamageFromRetains = value;
        }
    }


    protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike];


    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(7m, ValueProp.Move),
        new(_retainedIncreaseKey, 4m) // +4 per retain
    ];

    public override HashSet<CardKeyword> CanonicalKeywords => [CardKeyword.Retain];

    public override bool ShouldReceiveCombatHooks => true;
    

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    public override async Task AfterCardRetained(CardModel card)
    {
        // Only trigger for this specific instance of Windmill Strike
        if (card == this)
        {
            var increaseAmount = DynamicVars[_retainedIncreaseKey].BaseValue;

            DynamicVars.Damage.BaseValue += increaseAmount;
            ExtraDamageFromRetains += increaseAmount;
        }

        await Task.CompletedTask;
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m); // 7 → 10 base damage
        DynamicVars[_retainedIncreaseKey].UpgradeValueBy(1m); // 4 → 5 per retain
    }

    protected override void AfterDowngraded()
    {
        base.AfterDowngraded();
        DynamicVars.Damage.BaseValue += ExtraDamageFromRetains;
    }
}