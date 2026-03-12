using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace WatcherMod.Models.Cards;

public sealed class Indignation() : CardModel(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<VulnerablePower>(3m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<WrathStance>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var isInWrath = Owner.Creature.Powers.OfType<WrathStance>().Any();
        if (CombatState == null) return;
        if (isInWrath)
            await PowerCmd.Apply<VulnerablePower>(
                CombatState.Enemies,
                DynamicVars.Vulnerable.BaseValue,
                Owner.Creature,
                this
            );
        else
            // Enter Wrath
            await ChangeStanceCmd.Execute(Owner.Creature, ModelDb.Power<WrathStance>(), choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Vulnerable.UpgradeValueBy(2m);
    }
}