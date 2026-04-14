using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Watcher.Code.Extensions;

namespace Watcher.Code.Powers;

public class DevotionPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();


    public override async Task BeforeHandDrawLate(Player player, PlayerChoiceContext choiceContext,
        CombatState combatState)
    {
        await PowerCmd.Apply<MantraPower>(player.Creature, Amount, player.Creature, null);
        Flash();
    }
}