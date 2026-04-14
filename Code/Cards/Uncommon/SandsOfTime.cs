using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Watcher.Code.Abstract;
using Watcher.Code.Character;

namespace Watcher.Code.Cards.Uncommon;

[Pool(typeof(WatcherCardPool))]
public sealed class SandsOfTime : WatcherCardModel
{
    public SandsOfTime() : base(4, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(20, 6);
        WithKeywords(CardKeyword.Retain);
    }

    public override bool ShouldReceiveCombatHooks => true;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(choiceContext);
    }

    public override async Task AfterCardRetained(CardModel card)
    {
        if (card == this)
        {
            var currentCost = EnergyCost.GetWithModifiers(CostModifiers.Local);
            if (currentCost > 0) EnergyCost.SetThisCombat(currentCost - 1);
        }

        await Task.CompletedTask;
    }
}