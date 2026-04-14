using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Watcher.Code.Abstract;
using Watcher.Code.Character;

namespace Watcher.Code.Cards.Uncommon;

[Pool(typeof(WatcherCardPool))]
public sealed class Perseverance : WatcherCardModel
{
    private const string IncreaseKey = "Increase";

    private decimal _extraBlockFromRetains;

    public Perseverance() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithBlock(5, 2);
        WithVar(IncreaseKey, 2, 1);
        WithKeywords(CardKeyword.Retain);
    }

    private decimal ExtraBlockFromRetains
    {
        get => _extraBlockFromRetains;
        set
        {
            AssertMutable();
            _extraBlockFromRetains = value;
        }
    }

    public override bool ShouldReceiveCombatHooks => true;


    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
    }

    public override async Task AfterCardRetained(CardModel card)
    {
        if (card == this)
        {
            var increaseAmount = DynamicVars[IncreaseKey].BaseValue;

            DynamicVars.Block.BaseValue += increaseAmount;
            ExtraBlockFromRetains += increaseAmount;
        }

        await Task.CompletedTask;
    }

    protected override void AfterDowngraded()
    {
        base.AfterDowngraded();
        DynamicVars.Block.BaseValue += ExtraBlockFromRetains;
    }
}