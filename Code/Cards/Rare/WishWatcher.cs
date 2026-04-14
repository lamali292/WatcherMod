using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using Watcher.Code.Abstract;
using Watcher.Code.Cards.Token;
using Watcher.Code.Character;
using Watcher.Code.Powers;

namespace Watcher.Code.Cards.Rare;

[Pool(typeof(WatcherCardPool))]
public sealed class WishWatcher : WatcherCardModel
{
    public WishWatcher() : base(3, CardType.Skill, CardRarity.Rare, TargetType.None)
    {
        WithVars(new GoldVar(25).WithUpgrade(5));
        WithPower<StrengthPower>(3, 1);
        WithPower<PlatedArmorPower>(6, 2);
        WithKeywords(CardKeyword.Exhaust);
    }


    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var cardsToChoose = new CardModel[]
        {
            ModelDb.Card<BecomeAlmighty>(),
            ModelDb.Card<FameAndFortune>(),
            ModelDb.Card<LiveForever>()
        }.Select(e => (CardModel)e.MutableClone()).ToList();

        foreach (var c in cardsToChoose)
        {
            c.Owner = Owner;
            if (IsUpgraded)
                CardCmd.Upgrade(c);
        }

        var card = await CardSelectCmd.FromChooseACardScreen(
            choiceContext,
            cardsToChoose,
            Owner
        );

        if (card is WishableWatcherCard wish) await wish.OnWish(choiceContext, cardPlay);
    }
}