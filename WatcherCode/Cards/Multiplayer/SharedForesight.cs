using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Watcher.Code.Abstract;
using Watcher.Code.Character;
using Watcher.Code.Commands;
using Watcher.Code.Extensions;

namespace Watcher.Code.Cards.Multiplayer;

[Pool(typeof(WatcherCardPool))]
public class SharedForesight : WatcherCardModel
{
    public SharedForesight() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllAllies)
    {
        WithScry(3, 1);
        WithKeyword(CardKeyword.Exhaust);
    }

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        await MultiplayerCmd.Scry(ctx, this, Owner.AllTeammates, DynamicVars.Scry().IntValue);
    }
}
