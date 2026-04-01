using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Watcher.Code.Cards.CardModels;
using Watcher.Code.Character;
using Watcher.Code.Extensions;
using Watcher.Code.Powers;
using Watcher.Code.Stances;

namespace Watcher.Code.Cards.Common;

[Pool(typeof(WatcherCardPool))]
public sealed class Prostrate() : WatcherCardModel(0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override bool GainsBlock => true;


    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(4m, ValueProp.Move),
        new PowerVar<MantraPower>(2m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<MantraPower>(),
        HoverTipFactory.FromPower<DivinityStance>()
    ];

    

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<MantraPower>(
            Owner.Creature,
            DynamicVars["MantraPower"].IntValue,
            Owner.Creature,
            this
        );

        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["MantraPower"].UpgradeValueBy(1m);
    }
}