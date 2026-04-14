using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using Watcher.Code.Abstract;
using Watcher.Code.Cards.Token;
using Watcher.Code.Commands;

namespace Watcher.Code.Powers;

public class StudyPower : WatcherPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (Owner.Player == null || Owner.Player?.Creature.Side != side) return;
        await WatcherCmd.GiveCards<Insight>(Owner.Player, Amount, PileType.Draw, CardPilePosition.Random, animationStyle: CardPreviewStyle.MessyLayout);
    }
}