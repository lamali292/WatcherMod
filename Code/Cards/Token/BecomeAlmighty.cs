using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using Watcher.Code.Abstract;

namespace Watcher.Code.Cards.Token;

[Pool(typeof(TokenCardPool))]
public sealed class BecomeAlmighty : WishableWatcherCard
{
    public BecomeAlmighty() : base(-1, CardType.Power, CardRarity.Token, TargetType.None)
    {
        WithPower<StrengthPower>(3, 1);
    }

    public override async Task OnWish(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<StrengthPower>(this);
    }
    
}