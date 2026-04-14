using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using Watcher.Code.Abstract;
using Watcher.Code.Character;

namespace Watcher.Code.Cards.Common;

[Pool(typeof(WatcherCardPool))]
public sealed class BowlingBash : WatcherCardModel
{
    public BowlingBash() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(7, 3);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var combatState = cardPlay.Target?.CombatState;
        if (combatState == null) return;
        var enemyCount = combatState.Enemies.Count(e => NCombatRoom.Instance?.GetCreatureNode(e)?.Visible ?? true);
        await CommonActions.CardAttack(this, cardPlay)
            .WithHitCount(enemyCount)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }
}