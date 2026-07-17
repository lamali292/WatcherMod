using BaseLib.Cards.Variables;
using BaseLib.Commands;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Watcher.Code.Abstract;
using Watcher.Code.Character;
using Watcher.Code.Commands;
using Watcher.Code.Extensions;
using Watcher.Code.Keywords;

namespace Watcher.Code.Relics;

[Pool(typeof(WatcherRelicPool))]
public sealed class Melange : WatcherRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Shop;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new ScryVar(3)];


    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(WatcherKeywords.Scry)
    ];

    public override async Task AfterShuffle(PlayerChoiceContext choiceContext, Player shuffler)
    {
        await ScryCmd.Execute(choiceContext, shuffler, DynamicVars.Scry().IntValue);
    }
}