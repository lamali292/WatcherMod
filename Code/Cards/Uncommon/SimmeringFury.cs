using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using Watcher.Code.Cards.CardModels;
using Watcher.Code.Character;
using Watcher.Code.Extensions;
using Watcher.Code.Powers;
using Watcher.Code.Stances;

namespace Watcher.Code.Cards.Uncommon;

[Pool(typeof(WatcherCardPool))]
public sealed class SimmeringFury() : WatcherCardModel(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<DrawCardsNextTurnPower>(2m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<SimmeringRagePower>(),
        HoverTipFactory.FromPower<DrawCardsNextTurnPower>(),
        HoverTipFactory.FromPower<WrathStance>()
    ];

    

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<SimmeringRagePower>(Owner.Creature, 1, Owner.Creature, this);
        await PowerCmd.Apply<DrawCardsNextTurnPower>(Owner.Creature, DynamicVars["DrawCardsNextTurnPower"].IntValue,
            Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["DrawCardsNextTurnPower"].UpgradeValueBy(1);
    }
}