using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Watcher.Code.Abstract;
using Watcher.Code.Cards.Token;
using Watcher.Code.Character;
using Watcher.Code.Commands;

namespace Watcher.Code.Potions;

[Pool(typeof(WatcherPotionPool))]
public class BottledMiracle : WatcherPotionModel
{
    // The base amount of Miracles to add
    private const int BaseAmount = 2;

    public override PotionRarity Rarity => PotionRarity.Common;
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    public override TargetType TargetType => TargetType.AnyPlayer;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(BaseAmount)
    ];

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        if (target?.Player == null) return;
        var amount = DynamicVars.Cards.IntValue;
        await WatcherCmd.GiveCards<Miracle>(target.Player, amount, PileType.Hand);
    }
}