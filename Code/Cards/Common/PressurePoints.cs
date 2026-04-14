using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Watcher.Code.Character;
using Watcher.Code.Extensions;
using Watcher.Code.Powers;

namespace Watcher.Code.Cards.Common;

[Pool(typeof(WatcherCardPool))]
public sealed class PressurePoints() : CustomCardModel(1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<MarkPower>(8m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<MarkPower>()
    ];

    public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        // Apply Mark power
        await PowerCmd.Apply<MarkPower>(
            cardPlay.Target,
            DynamicVars["MarkPower"].IntValue,
            Owner.Creature,
            this
        );
        var combatState = cardPlay.Target.CombatState;
        var enemies = combatState!.Enemies.ToList();
        foreach (var enemy in enemies)
        {
            var markPower = enemy.GetPower<MarkPower>();
            if (markPower is { Amount: > 0 })
                await DamageCmd.Attack(markPower.Amount)
                    .FromCard(this)
                    .Targeting(enemy)
                    .Unpowered()
                    .Execute(choiceContext);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["MarkPower"].UpgradeValueBy(3m);
    }
}