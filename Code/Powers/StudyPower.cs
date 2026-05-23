using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using Watcher.Code.Abstract;
using Watcher.Code.Cards.Token;
using Watcher.Code.Commands;

namespace Watcher.Code.Powers;

public class StudyPower : WatcherPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;


    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<Insight>()];

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (Owner.Player == null || Owner.Player?.Creature.Side != side) return;
        await WatcherCmd.GiveCards<Insight>(Owner.Player, Amount, PileType.Draw, CardPilePosition.Random,
            animationStyle: CardPreviewStyle.MessyLayout);
    }
}