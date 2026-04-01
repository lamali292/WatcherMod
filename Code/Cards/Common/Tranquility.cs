using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using Watcher.Code.Cards.CardModels;
using Watcher.Code.Character;
using Watcher.Code.Commands;
using Watcher.Code.Extensions;
using Watcher.Code.Stances;

namespace Watcher.Code.Cards.Common;

[Pool(typeof(WatcherCardPool))]
public sealed class Tranquility() : WatcherCardModel(1, CardType.Skill, CardRarity.Common, TargetType.None)
{
    public override HashSet<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Retain,
        CardKeyword.Exhaust
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<CalmStance>()
    ];

    

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await StanceCmd.EnterCalm(Owner.Creature, cardPlay.Card);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}