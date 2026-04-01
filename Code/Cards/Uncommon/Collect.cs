using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using Watcher.Code.Cards.CardModels;
using Watcher.Code.Cards.Token;
using Watcher.Code.Character;
using Watcher.Code.Extensions;
using Watcher.Code.Powers;

namespace Watcher.Code.Cards.Uncommon;

[Pool(typeof(WatcherCardPool))]
public sealed class Collect() : WatcherCardModel(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override HashSet<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];


    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Miracle>(true)
    ];

    protected override bool HasEnergyCostX => true;
    

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var numOfXs = ResolveEnergyXValue();
        if (IsUpgraded) numOfXs++;

        await PowerCmd.Apply<CollectPower>(Owner.Creature, numOfXs, Owner.Creature, this);
    }
}