using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using Watcher.Code.Cards.CardModels;
using Watcher.Code.Character;
using Watcher.Code.Commands;
using Watcher.Code.Extensions;
using Watcher.Code.Stances;

namespace Watcher.Code.Cards.Uncommon;

[Pool(typeof(WatcherCardPool))]
public sealed class Indignation() : WatcherCardModel(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<VulnerablePower>(3m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<WrathStance>(),
        HoverTipFactory.FromPower<VulnerablePower>()
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
            await StanceCmd.EnterWrath(Owner.Creature, cardPlay.Card);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Vulnerable.UpgradeValueBy(2m);
    }
}