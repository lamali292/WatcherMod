using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Watcher.Code.Abstract;
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
        WithStanceTip<DivinityStance>();
        WithKeywords(CardKeyword.Exhaust);
        WithCostUpgradeBy(-1);
    }
    
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await CommonActions.CardAttack(this, cardPlay).WithHitFx("vfx/vfx_attack_slash").Execute(ctx);
        await StanceCmd.EnterDivinity(ctx, Owner, cardPlay.Card);
    }
}