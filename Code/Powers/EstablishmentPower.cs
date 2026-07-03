using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using Watcher.Code.Abstract;

namespace Watcher.Code.Powers;

public sealed class EstablishmentPower : WatcherPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CardKeyword.Retain)
    ];


    public override Task AfterFlush(PlayerChoiceContext choiceContext, Player player,
        IReadOnlyCollection<CardModel> flushedCards,
        IReadOnlyCollection<CardModel> retainedCards)
    {
        if (player.Creature != Owner) return Task.CompletedTask;
        foreach (var card in retainedCards)
        {
            card.EnergyCost.AddThisCombat(-Amount);
        }
            
        return Task.CompletedTask;
    }
/*
    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {

        if (side != Owner.Side || Owner.Player == null) return;
        if (Owner.GetPower<RetainHandPower>() == null) return;
        foreach (var card in PileType.Hand.GetPile(Owner.Player).Cards) card.EnergyCost.AddThisCombat(-Amount);
        await Task.CompletedTask;
    }
    */
}