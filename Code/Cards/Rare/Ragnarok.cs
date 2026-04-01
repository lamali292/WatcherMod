using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Watcher.Code.Cards.CardModels;
using Watcher.Code.Character;
using Watcher.Code.Extensions;

namespace Watcher.Code.Cards.Rare;

[Pool(typeof(WatcherCardPool))]
public sealed class Ragnarok() : WatcherCardModel(3, CardType.Attack, CardRarity.Rare, TargetType.RandomEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(5m, ValueProp.Move),
        new RepeatVar(5)
    ];

    

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState != null)
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .TargetingRandomOpponents(CombatState)
                .WithHitCount(DynamicVars.Repeat.IntValue)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1m);
        DynamicVars.Repeat.UpgradeValueBy(1);
    }
}