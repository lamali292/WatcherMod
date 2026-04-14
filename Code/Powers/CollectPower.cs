using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Watcher.Code.Cards.Token;
using Watcher.Code.Extensions;

namespace Watcher.Code.Powers;

public sealed class CollectPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, CombatState combatState)
    {
        var insightCard = CombatState.CreateCard<Miracle>(player);
        CardCmd.Upgrade(insightCard);
        // Add to hand at top position
        CardCmd.PreviewCardPileAdd(
            await CardPileCmd.AddGeneratedCardToCombat(
                insightCard,
                PileType.Hand,
                false,
                CardPilePosition.Top
            )
        );
        await PowerCmd.TickDownDuration(this);
    }
}