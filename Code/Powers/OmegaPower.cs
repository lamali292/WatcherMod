using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using Watcher.Code.Extensions;

namespace Watcher.Code.Powers;

public class OmegaPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        // Only trigger at end of owner's turn
        if (Owner.Side != side)
            return;

        decimal damage = Amount;
        if (damage <= 0)
            return;

        var enemies = CombatState
            .GetOpponentsOf(Owner)
            .Where(c => c.IsAlive);

        await CreatureCmd.Damage(
            choiceContext,
            enemies,
            damage,
            ValueProp.Unpowered,
            Owner
        );
        Flash();
    }
}