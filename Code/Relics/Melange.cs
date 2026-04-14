using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Watcher.Code.Character;
using Watcher.Code.Commands;
using Watcher.Code.Extensions;

namespace Watcher.Code.Relics;

[Pool(typeof(WatcherRelicPool))]
public sealed class Melange : CustomRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Shop;

    public override string PackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".RelicImagePath();

    public override async Task AfterShuffle(PlayerChoiceContext choiceContext, Player shuffler)
    {
        await ScryCmd.Execute(choiceContext, shuffler, 3);
    }
}