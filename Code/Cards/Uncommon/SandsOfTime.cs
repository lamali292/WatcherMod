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
public sealed class SandsOfTime() : WatcherCardModel(4, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(20, ValueProp.Move)
    ];

    public override HashSet<CardKeyword> CanonicalKeywords => [CardKeyword.Retain];

    public override bool ShouldReceiveCombatHooks => true;
    

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    public override async Task AfterCardRetained(CardModel card)
    {
        if (card == this)
        {
            var currentCost = EnergyCost.GetWithModifiers(CostModifiers.Local);
            // Reduce ONLY this card's cost (not all SandsOfTime copies!)
            if (currentCost > 0) EnergyCost.SetThisCombat(currentCost - 1);
        }

        await Task.CompletedTask;
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(6m);
    }
}