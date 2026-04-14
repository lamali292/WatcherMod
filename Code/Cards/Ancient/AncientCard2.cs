using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Watcher.Code.Cards.CardModels;
using Watcher.Code.Character;
using Watcher.Code.Commands;
using Watcher.Code.Stances;

namespace Watcher.Code.Cards.Ancient;

[Pool(typeof(WatcherCardPool))]
public sealed class AncientCard2 : WatcherCardModel
{
    public AncientCard2() : base(2, CardType.Attack, CardRarity.Ancient, TargetType.AnyEnemy)
    {
        WithDamage(12);
        WithTip(typeof(DivinityStance));
    }
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await StanceCmd.EnterDivinity(Owner.Creature, cardPlay.Card);
        await CommonActions.CardAttack(this, cardPlay).WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}