using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Watcher.Code.Abstract;
using Watcher.Code.Cards.Token;
using Watcher.Code.Character;
using Watcher.Code.Commands;

namespace Watcher.Code.Cards.Multiplayer;

[Pool(typeof(WatcherCardPool))]
public class GiftOfMiracles : WatcherCardModel
{
    public GiftOfMiracles() : base(1, CardType.Skill, CardRarity.Rare, TargetType.AllAllies)
    {
        WithCards(1, 1);
        WithTip(typeof(Miracle));
        WithKeyword(CardKeyword.Exhaust);
    }

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        foreach (var player in CombatState.GetTeammatesOf(Owner.Creature)
                     .Where(c => c.IsAlive && c != Owner.Creature).Select(c => c.Player).OfType<Player>())
            await WatcherCmd.GiveCards<Miracle>(player, DynamicVars.Cards.IntValue, PileType.Hand, upgraded: true);
    }
}
