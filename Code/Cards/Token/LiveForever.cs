using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using Watcher.Code.Abstract;
using Watcher.Code.Powers;

namespace Watcher.Code.Cards.Token;

[Pool(typeof(TokenCardPool))]
public sealed class LiveForever : WishableWatcherCard
{
    public LiveForever() : base(-1, CardType.Power, CardRarity.Token, TargetType.None)
    {
        WithPower<PlatedArmorPower>(6, 2);
    }

    public override async Task OnWish(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<PlatedArmorPower>(this);
    }
}