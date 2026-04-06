using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using Watcher.Code.Abstract;
using Watcher.Code.Character;
using Watcher.Code.Powers;
using Watcher.Code.Stances;

namespace Watcher.Code.Cards.Uncommon;

[Pool(typeof(WatcherCardPool))]
public sealed class SimmeringFury : WatcherCardModel
{
    public SimmeringFury() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithPower<DrawCardsNextTurnPower>(2, 1);
        WithPower<SimmeringRagePower>(1);
        WithStanceTip<WrathStance>();
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<SimmeringRagePower>(this);
        await CommonActions.ApplySelf<DrawCardsNextTurnPower>(this);
    }
}