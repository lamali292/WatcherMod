using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using Watcher.Code.Cards.CardModels;
using Watcher.Code.Extensions;

namespace Watcher.Code.Cards.Token;

[Pool(typeof(TokenCardPool))]
public sealed class Insight() : WatcherCardModel(0, CardType.Skill, CardRarity.Token, TargetType.None)
{
    public override CardPoolModel Pool => ModelDb.CardPool<TokenCardPool>();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(2) // Scry amount
    ];


    public override HashSet<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Retain,
        CardKeyword.Exhaust
    ];

    

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}