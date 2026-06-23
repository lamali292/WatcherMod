using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.ValueProps;
using Watcher.Code.Abstract;
using Watcher.Code.Character;

namespace Watcher.Code.Cards.Uncommon;

[Pool(typeof(WatcherCardPool))]
public sealed class Wallop : WatcherCardModel
{
    public Wallop() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(9, 3);
        WithTip(StaticHoverTip.Block);
    }


    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var command = await CommonActions.CardAttack(this, cardPlay).Execute(choiceContext);
        var sum = command.Results.SelectMany(e => e).Sum(damageResult => damageResult.UnblockedDamage);
        await CreatureCmd.GainBlock(
            Owner.Creature,
            sum,
            ValueProp.Move,
            null
        );
    }
}