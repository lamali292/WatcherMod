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
using Watcher.Code.Commands;
using Watcher.Code.Extensions;
using Watcher.Code.Keywords;

namespace Watcher.Code.Cards.Common;

[Pool(typeof(WatcherCardPool))]
public sealed class ThirdEye() : WatcherCardModel(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override bool GainsBlock => true;


    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(7m, ValueProp.Move),
        new CardsVar(3)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(WatcherKeywords.Scry)
    ];

    

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        await ScryCmd.Execute(choiceContext, Owner, DynamicVars.Cards.IntValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2m);
        DynamicVars.Cards.UpgradeValueBy(2);
    }
}