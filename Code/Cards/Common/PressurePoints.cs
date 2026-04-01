using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Watcher.Code.Cards.CardModels;
using Watcher.Code.Character;
using Watcher.Code.Extensions;
using Watcher.Code.Powers;

namespace Watcher.Code.Cards.Common;

[Pool(typeof(WatcherCardPool))]
public sealed class PressurePoints() : WatcherCardModel(1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<MarkPower>(8m),
        new CalculationBaseVar(8m),
        new ExtraDamageVar(1m),
        new CalculatedDamageVar(ValueProp.Unpowered | ValueProp.Unblockable)
            .WithMultiplier(static (_, target) => target?.GetPowerAmount<MarkPower>() ?? 0m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<MarkPower>()
    ];


    

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        await PowerCmd.Apply<MarkPower>(
            cardPlay.Target,
            DynamicVars["MarkPower"].IntValue,
            Owner.Creature,
            this
        );

        var enemies = cardPlay.Target.CombatState!.Enemies.ToList();
        foreach (var enemy in enemies)
        {
            var damage = ((CalculatedDamageVar)DynamicVars["CalculatedDamage"]).Calculate(enemy) - 8;
            if (damage <= 0) continue;
            await CreatureCmd.Damage(
                choiceContext,
                enemy,
                damage,
                ValueProp.Unpowered | ValueProp.Unblockable,
                this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["MarkPower"].UpgradeValueBy(3m);
    }
}