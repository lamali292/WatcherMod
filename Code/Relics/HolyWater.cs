using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Watcher.Code.Cards.Token;
using Watcher.Code.Character;
using Watcher.Code.Extensions;

namespace Watcher.Code.Relics;

[Pool(typeof(WatcherRelicPool))]
public sealed class HolyWater : CustomRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Ancient;


    protected override string BigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigRelicImagePath();
    public override string PackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.tres".TresRelicImagePath();

    protected override string PackedIconOutlinePath =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.tres".TresRelicImagePath();
    
    public override async Task BeforeHandDraw(
        Player player,
        PlayerChoiceContext choiceContext,
        CombatState combatState)
    {
        if (player != Owner || combatState.RoundNumber > 1) return;
        var miracles =
            new CardModel[]
            {
                combatState.CreateCard<Miracle>(Owner),
                combatState.CreateCard<Miracle>(Owner),
                combatState.CreateCard<Miracle>(Owner)
            };
        await CardPileCmd.AddGeneratedCardsToCombat(miracles, PileType.Hand, true);
        Flash();
    }
}