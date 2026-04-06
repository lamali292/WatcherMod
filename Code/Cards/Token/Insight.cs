using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using Watcher.Code.Abstract;

namespace Watcher.Code.Cards.Token;

[Pool(typeof(TokenCardPool))]
public sealed class Insight : WatcherCardModel
{
    public Insight() : base(0, CardType.Skill, CardRarity.Token, TargetType.None)
    {
        WithCards(2, 1);
        WithKeywords(CardKeyword.Retain, CardKeyword.Exhaust);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.Draw(this, choiceContext);
    }
}