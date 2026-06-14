using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using Watcher.Code.Abstract;
using Watcher.Code.Cards.Token;
using Watcher.Code.Character;

namespace Watcher.Code.Relics;

[Pool(typeof(WatcherRelicPool))]
public sealed class HolyWater : WatcherRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Ancient;


    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Miracle>()
    ];

    public override async Task BeforeHandDraw(
        Player player,
        PlayerChoiceContext choiceContext,
        ICombatState combatState)
    {
        if (player != Owner || Owner.PlayerCombatState is not { TurnNumber: 1 }) return;
        var miracles =
            new CardModel[]
            {
                combatState.CreateCard<Miracle>(Owner),
                combatState.CreateCard<Miracle>(Owner),
                combatState.CreateCard<Miracle>(Owner)
            };
        await CardPileCmd.AddGeneratedCardsToCombat(miracles, PileType.Hand, player);
        Flash();
    }
}