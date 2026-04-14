using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Watcher.Code.Abstract;
using Watcher.Code.Character;

namespace Watcher.Code.Cards.Uncommon;

[Pool(typeof(WatcherCardPool))]
public sealed class SignatureMove : WatcherCardModel
{
    public SignatureMove() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(30, 10);
    }


    protected override bool IsPlayable
    {
        get
        {
            if (Owner.PlayerCombatState == null) return false;
            var hand = Owner.PlayerCombatState.Hand;
            var attackCount = hand.Cards.Count(card => card.Type == CardType.Attack && card != this);
            return attackCount == 0;
        }
    }


    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(choiceContext);
    }
}