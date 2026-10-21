using BaseLib.Hooks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using Watcher.Code.Abstract;

namespace Watcher.Code.Powers;

public sealed class ClairvoyancePower : WatcherPowerModel, IAfterScryed
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(BaseLibTip.Scry)
    ];

    public async Task AfterScryed(PlayerChoiceContext ctx, Player player, int scryAmount, int discardAmount,
        List<CardModel> seen, List<CardModel> discarded)
    {
        if (player != Owner.Player)
            return;

        Flash();
        await CardPileCmd.Draw(ctx, Amount, player);
    }
}
