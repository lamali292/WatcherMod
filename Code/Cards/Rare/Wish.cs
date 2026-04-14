using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using Watcher.Code.Cards.CardModels;
using Watcher.Code.Cards.Token;
using Watcher.Code.Character;
using Watcher.Code.Extensions;

namespace Watcher.Code.Cards.Rare;

[Pool(typeof(WatcherCardPool))]
public sealed class Wish() : CustomCardModel(3, CardType.Skill, CardRarity.Rare, TargetType.None)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new GoldVar(25), new PowerVar<StrengthPower>(3), new PowerVar<PlatingPower>(6)];


    public override HashSet<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>(),
        HoverTipFactory.FromPower<PlatingPower>()
    ];

    public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var cardsToChoose = new List<CardModel>
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

        if (card is IWishable wish) await wish.OnWish(choiceContext, cardPlay);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Gold.UpgradeValueBy(5);
        DynamicVars["PlatingPower"].UpgradeValueBy(2);
        DynamicVars.Strength.UpgradeValueBy(1);
    }
}