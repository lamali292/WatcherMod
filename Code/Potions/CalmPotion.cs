using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using Watcher.Code.Abstract;
using Watcher.Code.Character;
using Watcher.Code.Commands;
using Watcher.Code.Stances;

namespace Watcher.Code.Potions;

[Pool(typeof(WatcherPotionPool))]
public class CalmPotion : WatcherPotionModel
{
    public override PotionRarity Rarity => PotionRarity.Uncommon;
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    public override TargetType TargetType => TargetType.AnyPlayer;

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<CalmStance>()
    ];

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        if (target == null) return;
        await StanceCmd.EnterCalm(target, null);
    }
}