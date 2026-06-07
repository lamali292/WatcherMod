using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Watcher.Code.Abstract;
using Watcher.Code.Character;
using Watcher.Code.Powers;
using Watcher.Code.Stances;

namespace Watcher.Code.Cards.Rare;

[Pool(typeof(WatcherCardPool))]
public sealed class Brilliance : WatcherCardModel
{
    public Brilliance() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithCalculatedDamage(12, MantraGainedThisCombat, ValueProp.Move, 4);
        WithTip(typeof(MantraPower));
        WithStanceTip<DivinityStance>();
    }

    public override bool ShouldReceiveCombatHooks => true;

    private static decimal MantraGainedThisCombat(CardModel card, Creature? creature)
        => CombatManager.Instance.History.Entries.OfType<PowerReceivedEntry>()
            .Where(e => e.Power is MantraPower && e.Applier?.Player == card.Owner && e.Amount > 0)
            .Sum(e => e.Amount);

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        await CommonActions.CardAttack(this, play).WithHitFx("vfx/vfx_attack_slash").Execute(context);
    }
}