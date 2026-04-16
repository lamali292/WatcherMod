using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Watcher.Code.Abstract;
using Watcher.Code.Cards.Token;
using Watcher.Code.Character;
using Watcher.Code.Commands;

namespace Watcher.Code.Cards.Rare;

[Pool(typeof(WatcherCardPool))]
public sealed class DeusExMachina : WatcherCardModel
{
    public DeusExMachina() : base(-1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithKeywords(CardKeyword.Unplayable);
        WithCards(2, 1);
        WithTip(typeof(Miracle));
        WithTip(CardKeyword.Exhaust);
    }


    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel drawnCard, bool fromHandDraw)
    {
        if (drawnCard != this) return;
        await WatcherCmd.GiveCards<Miracle>(Owner, DynamicVars.Cards.IntValue, PileType.Hand, animationTime: 0.1f);
        await CardCmd.Exhaust(choiceContext, this);
    }
}