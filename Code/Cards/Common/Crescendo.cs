using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Watcher.Code.Abstract;
using Watcher.Code.Character;
using Watcher.Code.Commands;
using Watcher.Code.Stances;

namespace Watcher.Code.Cards.Common;

[Pool(typeof(WatcherCardPool))]
public sealed class Crescendo : WatcherCardModel
{
    public Crescendo() : base(1, CardType.Skill, CardRarity.Common, TargetType.None)
    {
        WithStanceTip<WrathStance>();
        WithKeywords(CardKeyword.Retain, CardKeyword.Exhaust);
        WithCostUpgradeBy(-1);
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await StanceCmd.EnterWrath(ctx, Owner, cardPlay.Card);
    }
}