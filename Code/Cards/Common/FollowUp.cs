using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Watcher.Code.Abstract;
using Watcher.Code.Character;

namespace Watcher.Code.Cards.Common;

[Pool(typeof(WatcherCardPool))]
public sealed class FollowUp : WatcherCardModel
{
    public FollowUp() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(7, 4);
    }

    protected override bool ShouldGlowGoldInternal => WasLastCardPlayedAttack;

    private bool WasLastCardPlayedAttack
    {
        get
        {
            var lastCardEntry = CombatManager.Instance.History.CardPlaysStarted
                .LastOrDefault(e =>
                    e.CardPlay.Card.Owner == Owner &&
                    e.CardPlay.Card != this);

            if (lastCardEntry == null) return false;

            return lastCardEntry.CardPlay.Card.Type == CardType.Attack;
        }
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        if (WasLastCardPlayedAttack) Owner.PlayerCombatState!.GainEnergy(1);
    }
}