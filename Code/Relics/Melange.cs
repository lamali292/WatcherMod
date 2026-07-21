using BaseLib.Cards.Variables;
using BaseLib.Commands;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Watcher.Code.Abstract;
using Watcher.Code.Character;

namespace Watcher.Code.Relics;

[Pool(typeof(WatcherRelicPool))]
public sealed class Melange : WatcherRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Shop;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new ScryVar(3)];

    public override async Task AfterShuffle(PlayerChoiceContext choiceContext, Player shuffler)
    {
        if (shuffler != Owner) return;
        await ScryCmd.Execute(choiceContext, shuffler, DynamicVars.Scry().IntValue);
    }
}