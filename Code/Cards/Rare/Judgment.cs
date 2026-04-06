using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Watcher.Code.Abstract;
using Watcher.Code.Character;

namespace Watcher.Code.Cards.Rare;

[Pool(typeof(WatcherCardPool))]
public sealed class Judgment : WatcherCardModel
{
    public Judgment() : base(1, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithVar("DamageThreshold", 30, 10);
    }


    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target == null)
            return;

        var target = cardPlay.Target;
        if (target.CurrentHp <= DynamicVars["DamageThreshold"].IntValue)
        {
            target.SetCurrentHpInternal(0);
            await CreatureCmd.Kill(target);
        }
    }
}