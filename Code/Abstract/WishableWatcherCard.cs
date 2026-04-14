using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
namespace Watcher.Code.Abstract;

// New abstract base
public abstract class WishableWatcherCard : WatcherCardModel
{
    protected WishableWatcherCard(int cost, CardType type, CardRarity rarity, TargetType target)
        : base(cost, type, rarity, target) { }

    public abstract Task OnWish(PlayerChoiceContext choiceContext, CardPlay cardPlay);

    protected override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        => OnWish(choiceContext, cardPlay);
}