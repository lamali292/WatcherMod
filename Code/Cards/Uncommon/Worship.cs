using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Watcher.Code.Abstract;
using Watcher.Code.Character;
using Watcher.Code.Powers;
using Watcher.Code.Stances;

namespace Watcher.Code.Cards.Uncommon;

[Pool(typeof(WatcherCardPool))]
public sealed class Worship : WatcherCardModel
{
    public Worship() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithPower<MantraPower>(5);
        WithStanceTip<DivinityStance>();
        WithKeyword(CardKeyword.Retain, UpgradeType.Add);
    }


    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<MantraPower>(this);
    }
}