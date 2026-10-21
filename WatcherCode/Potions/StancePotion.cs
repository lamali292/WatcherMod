using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using Watcher.Code.Abstract;
using Watcher.Code.Cards.Token;
using Watcher.Code.Character;
using Watcher.Code.Commands;
using Watcher.Code.Core;
using Watcher.Code.Stances;

namespace Watcher.Code.Potions;

[Pool(typeof(WatcherPotionPool))]
public class StancePotion : WatcherPotionModel
{
    public override PotionRarity Rarity => PotionRarity.Uncommon;
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    public override TargetType TargetType => TargetType.AnyPlayer;

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        WatcherHoverTipFactory.FromStance<CalmStance>(),
        WatcherHoverTipFactory.FromStance<WrathStance>()
    ];

    protected override async Task OnUse(PlayerChoiceContext ctx, Creature? target)
    {
        if (target?.Player == null) return;

        var choices = new CardModel[]
        {
            ModelDb.Card<EnterCalm>(),
            ModelDb.Card<EnterWrath>()
        }.Select(e => (CardModel)e.MutableClone()).ToList();
        foreach (var c in choices) c.Owner = Owner;

        var chosen = await CardSelectCmd.FromChooseACardScreen(ctx, choices, Owner);
        switch (chosen)
        {
            case EnterCalm:
                await StanceCmd.EnterCalm(ctx, target.Player, null);
                break;
            case EnterWrath:
                await StanceCmd.EnterWrath(ctx, target.Player, null);
                break;
        }
    }
}
