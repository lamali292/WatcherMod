using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using Watcher.Code.Abstract;
using Watcher.Code.Character;
using Watcher.Code.Events;
using Watcher.Code.Keywords;

namespace Watcher.Code.Relics;

[Pool(typeof(WatcherRelicPool))]
public sealed class GoldenEye : WatcherRelicModel, IModifyScryAmount
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(WatcherKeywords.Scry)
    ];

    public int ModifyScryAmount(Player player, int amount)
    {
        return player == Owner ? amount + 2 : amount;
    }

    public Task AfterModifyingScryAmount(PlayerChoiceContext ctx, Player player, int originalAmount, int modifiedAmount)
    {
        Flash();
        return Task.CompletedTask;
    }
}