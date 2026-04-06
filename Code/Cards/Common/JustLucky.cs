using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Watcher.Code.Abstract;
using Watcher.Code.Character;
using Watcher.Code.Commands;
using Watcher.Code.Keywords;

namespace Watcher.Code.Cards.Common;

[Pool(typeof(WatcherCardPool))]
public sealed class JustLucky : WatcherCardModel
{
    public JustLucky() : base(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithCards(1, 1);
        WithBlock(2, 1);
        WithDamage(3, 1);
        WithTip(WatcherKeywords.Scry);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await ScryCmd.Execute(choiceContext, Owner, DynamicVars.Cards.IntValue);
        await CommonActions.CardAttack(this, cardPlay)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }
}