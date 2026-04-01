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
using Watcher.Code.Commands;
using Watcher.Code.Extensions;
using Watcher.Code.Stances;

namespace Watcher.Code.Cards.Ancient;

[Pool(typeof(WatcherCardPool))]
public sealed class AncientCard2() : WatcherCardModel(1, CardType.Attack, CardRarity.Ancient, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(11m, ValueProp.Move)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<WrathStance>()
    ];

    

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await StanceCmd.EnterWrath(Owner.Creature, cardPlay.Card);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}