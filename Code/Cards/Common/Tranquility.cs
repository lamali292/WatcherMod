using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Watcher.Code.Abstract;
using Watcher.Code.Character;
using Watcher.Code.Commands;
using Watcher.Code.Stances;

namespace Watcher.Code.Cards.Common;

[Pool(typeof(WatcherCardPool))]
public sealed class Tranquility : WatcherCardModel
{
    public Tranquility() : base(1, CardType.Skill, CardRarity.Common, TargetType.None)
    {
        WithKeywords(CardKeyword.Retain, CardKeyword.Exhaust);
        WithStanceTip<CalmStance>();
        WithCostUpgradeBy(-1);
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await StanceCmd.EnterCalm(ctx, Owner, cardPlay.Card);
    }
}