using BaseLib.Commands;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Watcher.Code.Abstract;
using Watcher.Code.Character;

namespace Watcher.Code.Cards.Common;

[Pool(typeof(WatcherCardPool))]
public sealed class ThirdEye : WatcherCardModel
{
    public ThirdEye() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(7, 2);
        WithScry(3, 2);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await ScryCmd.Execute(choiceContext, this);
    }
}