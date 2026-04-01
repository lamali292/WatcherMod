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
public sealed class Perseverance() : WatcherCardModel(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    private const string _increaseKey = "Increase";

    private decimal _extraBlockFromRetains;

    private decimal ExtraBlockFromRetains
    {
        get => _extraBlockFromRetains;
        set
        {
            AssertMutable();
            _extraBlockFromRetains = value;
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(5, ValueProp.Move),
        new(_increaseKey, 2m)
    ];

    public override HashSet<CardKeyword> CanonicalKeywords => [CardKeyword.Retain];

    public override bool ShouldReceiveCombatHooks => true;
    

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(
            Owner.Creature,
            DynamicVars.Block,
            cardPlay
        );
    }

    public override async Task AfterCardRetained(CardModel card)
    {
        // Only trigger if THIS specific card instance was retained
        if (card == this)
        {
            // Buff ONLY this card (not all Perseverance copies!)
            var increaseAmount = DynamicVars[_increaseKey].BaseValue;

            DynamicVars.Block.BaseValue += increaseAmount;
            ExtraBlockFromRetains += increaseAmount;
        }

        await Task.CompletedTask;
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2m); // 5 → 7
        DynamicVars[_increaseKey].UpgradeValueBy(1m); // 2 → 3
    }

    protected override void AfterDowngraded()
    {
        base.AfterDowngraded();
        DynamicVars.Block.BaseValue += ExtraBlockFromRetains;
    }
}