using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using WatcherMod.Commands;

namespace WatcherMod.Relics;

public sealed class Melange : RelicModel
{
    public override RelicRarity Rarity => RelicRarity.Shop;

    public override async Task AfterShuffle(PlayerChoiceContext choiceContext, Player shuffler)
    {
        await ScryCmd.Execute(choiceContext, shuffler, 3);
    }
}