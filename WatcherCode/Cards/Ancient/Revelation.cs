using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Watcher.Code.Abstract;
using Watcher.Code.Character;
using Watcher.Code.Commands;
using Watcher.Code.Events;
using Watcher.Code.Stances;

namespace Watcher.Code.Cards.Ancient;

// Orobas (Archaic Tooth) upgrade of Vigilance: cheaper and it keeps coming back. Leaving Calm pays out its energy,
// and every exit returns this card, so Calm -> another stance -> Calm becomes a loop.
[Pool(typeof(WatcherCardPool))]
public sealed class Revelation : WatcherCardModel, IOnStanceChange
{
    public Revelation() : base(1, CardType.Skill, CardRarity.Ancient, TargetType.Self)
    {
        WithBlock(10, 4);
        WithStanceTip<CalmStance>();
    }

    public async Task OnStanceChange(PlayerChoiceContext ctx, Player player, WatcherStanceModel oldStance,
        WatcherStanceModel newStance)
    {
        if (player != Owner || oldStance is not CalmStance || Pile?.Type != PileType.Discard) return;
        await CardPileCmd.Add(this, PileType.Hand);
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await StanceCmd.EnterCalm(ctx, Owner, cardPlay.Card);
    }
}
