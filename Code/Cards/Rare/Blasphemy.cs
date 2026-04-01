using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using Watcher.Code.Cards.CardModels;
using Watcher.Code.Character;
using Watcher.Code.Commands;
using Watcher.Code.Extensions;
using Watcher.Code.Powers;
using Watcher.Code.Stances;

namespace Watcher.Code.Cards.Rare;

[Pool(typeof(WatcherCardPool))]
public sealed class Blasphemy() : WatcherCardModel(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<DivinityStance>()
    ];

    public override HashSet<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<BlasphemerPower>(Owner.Creature, 1, Owner.Creature, this);
        await StanceCmd.EnterDivinity(Owner.Creature, cardPlay.Card);
    }


    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}