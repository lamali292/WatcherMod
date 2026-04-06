using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Watcher.Code.Abstract;
using Watcher.Code.Character;
using Watcher.Code.Commands;

namespace Watcher.Code.Relics;

[Pool(typeof(WatcherRelicPool))]
public sealed class Melange : WatcherRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Shop;

    public override async Task AfterShuffle(PlayerChoiceContext choiceContext, Player shuffler)
    {
        await ScryCmd.Execute(choiceContext, shuffler, 3);
    }
}