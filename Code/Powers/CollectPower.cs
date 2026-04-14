using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Watcher.Code.Abstract;
using Watcher.Code.Cards.Token;
using Watcher.Code.Extensions;

namespace Watcher.Code.Powers;

public sealed class CollectPower : WatcherPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, CombatState combatState)
    {
        if (player != Owner.Player)
            return;
        var insightCard = CombatState.CreateCard<Miracle>(player);
        CardCmd.Upgrade(insightCard);
        // Add to hand at top position
        CardCmd.PreviewCardPileAdd(
            await CardPileCmd.AddGeneratedCardToCombat(
                insightCard,
                PileType.Hand,
                true,
                CardPilePosition.Top
            )
        );
        await PowerCmd.TickDownDuration(this);
    }
}