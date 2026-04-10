using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Watcher.Code.Abstract;
using Watcher.Code.Character;
using Watcher.Code.Events;
using Watcher.Code.Keywords;

namespace Watcher.Code.Cards.Uncommon;

[Pool(typeof(WatcherCardPool))]
public sealed class Weave : WatcherCardModel, IOnScryed
{
    public Weave() : base(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(4, 2);
        WithTip(WatcherKeywords.Scry);
    }

    public async Task OnScryed(PlayerChoiceContext ctx, Player player, int amount, int discardedAmount)
    {
        if (player != Owner)
            return;

        // Check if this card is in discard pile
        var discardPile = PileType.Discard.GetPile(player);
        if (discardPile.Cards.Contains(this)) await CardPileCmd.Add(this, PileType.Hand);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        await CommonActions.CardAttack(this, cardPlay).Execute(choiceContext);
    }

   
}