using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Watcher.Code.Abstract;
using Watcher.Code.Character;
using Watcher.Code.Commands;

namespace Watcher.Code.Relics;

[Pool(typeof(WatcherRelicPool))]
public sealed class TeardropLocket : WatcherRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext ctx, CombatState combatState)
    {
        if (player != Owner || combatState.RoundNumber > 1) return; 
        await StanceCmd.EnterCalm(ctx, Owner, null);
        Flash();
    }
}
