using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using Watcher.Code.Cards.CardModels;
using Watcher.Code.Character;
using Watcher.Code.Extensions;
using Watcher.Code.Powers;
using Watcher.Code.Stances;

namespace Watcher.Code.Cards.Uncommon;

[Pool(typeof(WatcherCardPool))]
public sealed class Rushdown() : WatcherCardModel(1, CardType.Power, CardRarity.Uncommon, TargetType.None)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<WrathStance>()
    ];

    

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<RushdownPower>(Owner.Creature, 2, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}