using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Watcher.Code.Cards.Rare;
using Watcher.Code.Extensions;

namespace Watcher.Code.Powers;

public class DevotionPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    public override string CustomBigIconPath => CustomPackedIconPath;


    public override async Task BeforeHandDrawLate(Player player, PlayerChoiceContext choiceContext,
        CombatState combatState)
    {
        if (!player.Creature.HasPower<DevotionPower>())
            return;

        await PowerCmd.Apply<MantraPower>(player.Creature, Amount, player.Creature, ModelDb.Card<Devotion>());
        Flash();
    }
}