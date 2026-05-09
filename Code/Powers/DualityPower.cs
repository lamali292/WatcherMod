using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using Watcher.Code.Abstract;

namespace Watcher.Code.Powers;

// Bookkeeping debuff for the Duality relic. Each stack represents one point of Dexterity granted
// earlier in the turn that must be subtracted at turn end, making the relic's Dexterity gain temporary.
public sealed class DualityPower : WatcherPowerModel
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        // Only fire on the player's own turn end; Amount<=0 guards against stacking with no payload.
        if (Owner.Player?.Creature.Side != side || Amount <= 0) return;

        // Strip exactly the Dexterity granted by Duality this turn, then clean up so the next turn starts fresh.
        await PowerCmd.Apply<DexterityPower>(choiceContext, Owner.Player.Creature, -Amount, Owner.Player.Creature, null);
        Flash();
        if (Owner.IsAlive) RemoveInternal();
    }
}
