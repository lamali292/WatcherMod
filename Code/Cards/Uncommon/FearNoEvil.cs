using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using Watcher.Code.Abstract;
using Watcher.Code.Character;
using Watcher.Code.Commands;
using Watcher.Code.Stances;

namespace Watcher.Code.Cards.Uncommon;

[Pool(typeof(WatcherCardPool))]
public sealed class FearNoEvil : WatcherCardModel
{
    public FearNoEvil() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(8, 3);
        WithStanceTip<CalmStance>();
    }


    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        // Check if the enemy intends to attack
        var hasAttackIntent = cardPlay.Target?.Monster?.NextMove.Intents
            .Any(intent => intent.IntentType == IntentType.Attack) ?? false;
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        if (hasAttackIntent) await StanceCmd.EnterCalm(ctx, Owner, cardPlay.Card);
    }
}