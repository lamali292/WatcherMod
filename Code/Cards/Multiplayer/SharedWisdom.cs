using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Watcher.Code.Abstract;
using Watcher.Code.Character;
using Watcher.Code.Commands;
using Watcher.Code.Stances;

namespace Watcher.Code.Cards.Multiplayer;

[Pool(typeof(WatcherCardPool))]
public class SharedWisdom : WatcherCardModel
{
    public SharedWisdom() : base(3, CardType.Skill, CardRarity.Rare, TargetType.AnyAlly)
    {
        WithStanceTip<DivinityStance>();
        WithKeyword(CardKeyword.Ethereal, UpgradeType.Remove);
        WithKeyword(CardKeyword.Exhaust);
    }

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Target?.Player == null) return;
        await StanceCmd.EnterDivinity(ctx, cardPlay.Target.Player, this);
    }
}