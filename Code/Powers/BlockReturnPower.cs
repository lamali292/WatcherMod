using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Watcher.Code.Extensions;

namespace Watcher.Code.Powers;

public sealed class BlockReturnPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;
    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    public override string CustomBigIconPath => CustomPackedIconPath;
    
    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        var blockReturnPower = this;
        if (target != blockReturnPower.Owner || dealer == null || dealer == blockReturnPower.Owner || result.UnblockedDamage <= 0)
            return;
        var player = dealer.Player ?? blockReturnPower.Applier?.Player;
        if (player == null)
            return;
        await CreatureCmd.GainBlock(player.Creature, blockReturnPower.Amount, ValueProp.Unpowered, null);
    }
}