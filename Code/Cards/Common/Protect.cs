using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Watcher.Code.Cards.CardModels;
using Watcher.Code.Character;
using Watcher.Code.Extensions;

namespace Watcher.Code.Cards.Common;

[Pool(typeof(WatcherCardPool))]
public sealed class Protect() : WatcherCardModel(2, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override bool GainsBlock => true;

    public override HashSet<CardKeyword> CanonicalKeywords => [CardKeyword.Retain];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(12m, ValueProp.Move)];
    

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(4m);
    }
}