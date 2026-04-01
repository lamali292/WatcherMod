using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Watcher.Code.Cards.CardModels;
using Watcher.Code.Character;
using Watcher.Code.Extensions;
using Watcher.Code.Powers;

namespace Watcher.Code.Cards.Ancient;

[Pool(typeof(WatcherCardPool))]
public sealed class AncientCard() : WatcherCardModel(2, CardType.Power, CardRarity.Ancient, TargetType.None)
{
    

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<DisciplePower>(Owner.Creature, 1, Owner.Creature, this);
    }


    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
    }
}