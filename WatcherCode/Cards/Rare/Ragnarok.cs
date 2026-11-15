using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Watcher.Code.Abstract;
using Watcher.Code.Character;

namespace Watcher.Code.Cards.Rare;

[Pool(typeof(WatcherCardPool))]
public sealed class Ragnarok : WatcherCardModel
{
    public Ragnarok() : base(3, CardType.Attack, CardRarity.Rare, TargetType.RandomEnemy)
    {
        WithDamage(5, 1);
        WithVars(new RepeatVar(5).WithUpgrade(1));
    }


    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // Lightning strike on each struck enemy, like Ragnarok's divine bolts in Slay the Spire 1.
        await CommonActions.CardAttack(this, cardPlay)
            .WithHitCount(DynamicVars.Repeat.IntValue)
            .WithHitFx("vfx/vfx_attack_lightning")
            .Execute(choiceContext);
    }
}