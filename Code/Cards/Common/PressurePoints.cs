using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using Watcher.Code.Abstract;
using Watcher.Code.Character;
using Watcher.Code.Powers;

namespace Watcher.Code.Cards.Common;

[Pool(typeof(WatcherCardPool))]
public sealed class PressurePoints : WatcherCardModel
{
    public PressurePoints() : base(1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithPower<MarkPower>(8, 3);
    }

    private static decimal Calc(Creature? target)
    {
        return target?.GetPowerAmount<MarkPower>() ?? 0m;
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await CommonActions.Apply<MarkPower>(cardPlay.Target, this);
        var enemies = cardPlay.Target.CombatState!.Enemies.ToList();
        foreach (var enemy in enemies)
        {
            var damage = Calc(enemy);
            if (damage <= 0) continue;
            await CreatureCmd.Damage(
                choiceContext,
                enemy,
                damage,
                ValueProp.Unpowered | ValueProp.Unblockable,
                this);
        }
    }
}