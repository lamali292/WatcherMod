using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Watcher.Code.Abstract;
using Watcher.Code.Character;
using Watcher.Code.Powers;

namespace Watcher.Code.Cards.Uncommon;

[Pool(typeof(WatcherCardPool))]
public sealed class TalkToTheHand : WatcherCardModel
{
    public TalkToTheHand() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(5, 2);
        WithPower<BlockReturnPower>(2, 1, false);
        WithKeywords(CardKeyword.Exhaust);
    }


    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await CommonActions.Apply<BlockReturnPower>(ctx, cardPlay.Target, this);
    }
}