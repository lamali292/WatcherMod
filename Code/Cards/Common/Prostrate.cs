using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Watcher.Code.Abstract;
using Watcher.Code.Character;
using Watcher.Code.Powers;
using Watcher.Code.Stances;

namespace Watcher.Code.Cards.Common;

[Pool(typeof(WatcherCardPool))]
public sealed class Prostrate : WatcherCardModel
{
    public Prostrate() : base(0, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(4);
        WithPower<MantraPower>(2, 1);
        WithStanceTip<DivinityStance>();
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<MantraPower>(this);
        await CommonActions.CardBlock(this, cardPlay);
    }
}