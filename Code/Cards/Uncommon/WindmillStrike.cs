using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Watcher.Code.Abstract;
using Watcher.Code.Character;

namespace Watcher.Code.Cards.Uncommon;

[Pool(typeof(WatcherCardPool))]
public sealed class WindmillStrike : WatcherCardModel
{
    private const string RetainedIncreaseKey = "RetainIncrease";

    private decimal _extraDamageFromRetains;

    public WindmillStrike() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithTags(CardTag.Strike);
        WithDamage(7, 3);
        WithVar(RetainedIncreaseKey, 4, 1);
        WithKeywords(CardKeyword.Retain);
    }

    private decimal ExtraDamageFromRetains
    {
        get => _extraDamageFromRetains;
        set
        {
            AssertMutable();
            _extraDamageFromRetains = value;
        }
    }

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
        if (card == this)
        {
            var increaseAmount = DynamicVars[RetainedIncreaseKey].BaseValue;

            DynamicVars.Damage.BaseValue += increaseAmount;
            ExtraDamageFromRetains += increaseAmount;
        }

        await Task.CompletedTask;
    }

    protected override void AfterDowngraded()
    {
        base.AfterDowngraded();
        DynamicVars.Damage.BaseValue += ExtraDamageFromRetains;
    }
}