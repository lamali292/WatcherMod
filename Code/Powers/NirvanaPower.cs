using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Watcher.Code.Abstract;
using Watcher.Code.Events;
using Watcher.Code.Keywords;

namespace Watcher.Code.Powers;

public sealed class NirvanaPower : WatcherPowerModel, IAfterScryed
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;


    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(StaticHoverTip.Block),
        HoverTipFactory.FromKeyword(WatcherKeywords.Scry)
    ];

    public async Task AfterScryed(PlayerChoiceContext ctx, Player player, int scryAmount, int discardedAmount, IEnumerable<CardModel> discarded)
    {
        if (player != Owner.Player)
            return;

        await CreatureCmd.GainBlock(
            Owner,
            Amount,
            ValueProp.Unpowered,
            null
        );
        Flash();
    }
}