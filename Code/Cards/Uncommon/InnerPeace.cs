using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Watcher.Code.Abstract;
using Watcher.Code.Character;
using Watcher.Code.Commands;
using Watcher.Code.Extensions;
using Watcher.Code.Stances;

namespace Watcher.Code.Cards.Uncommon;

[Pool(typeof(WatcherCardPool))]
public sealed class InnerPeace : WatcherCardModel
{
    public InnerPeace() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithCards(3, 1);
        WithStanceTip<CalmStance>();
    }


    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var isInCalm = Owner.IsInWatcherStance<CalmStance>();
        if (isInCalm)
            await CommonActions.Draw(this, ctx);
        else
            await StanceCmd.EnterCalm(ctx, Owner, cardPlay.Card);
    }
}