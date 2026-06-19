using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Watcher.Code.Abstract;
using Watcher.Code.Character;

namespace Watcher.Code.Cards.Rare;

[Pool(typeof(WatcherCardPool))]
public sealed class SpiritShield : WatcherCardModel
{
    public SpiritShield() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithCalculatedBlock(0, 3, Calc, ValueProp.Move, 0, 1);
    }

    private static decimal Calc(CardModel cardModel, Creature? creature)
    {
        return cardModel.Owner.PlayerCombatState?.Hand.Cards.Count(e => e != cardModel) ?? 0;
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
    }
}