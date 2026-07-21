using BaseLib.Cards.Variables;
using BaseLib.Extensions;
using BaseLib.Hooks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Watcher.Code.Abstract;
using Watcher.Code.Character;

namespace Watcher.Code.Relics;

[Pool(typeof(WatcherRelicPool))]
public sealed class GoldenEye : WatcherRelicModel, IModifyScryAmount
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new ScryVar(2)];
    
    public int ModifyScryAmount(Player player, int amount)
    {
        return player == Owner ? amount + DynamicVars.Scry().IntValue : amount;
    }

    public Task AfterModifyingScryAmount(PlayerChoiceContext ctx, Player player, int originalAmount, int modifiedAmount)
    {
        Flash();
        return Task.CompletedTask;
    }
}