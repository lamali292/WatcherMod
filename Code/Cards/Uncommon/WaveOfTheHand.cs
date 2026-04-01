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

namespace Watcher.Code.Cards.Uncommon;

[Pool(typeof(WatcherCardPool))]
public sealed class WaveOfTheHand() : WatcherCardModel(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<WaveOfTheHandPower>(1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<WaveOfTheHandPower>(),
        HoverTipFactory.FromPower<WeakPower>(),
        HoverTipFactory.Static(StaticHoverTip.Block)
    ];


    


    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<WaveOfTheHandPower>(Owner.Creature, DynamicVars["WaveOfTheHandPower"].IntValue,
            Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["WaveOfTheHandPower"].UpgradeValueBy(1);
    }
}