using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Watcher.Code.Abstract;
using Watcher.Code.Character;
using Watcher.Code.Powers;

namespace Watcher.Code.Cards.Rare;

[Pool(typeof(WatcherCardPool))]
public sealed class DevaForm : WatcherCardModel
{
    public DevaForm() : base(3, CardType.Power, CardRarity.Rare, TargetType.None)
    {
        WithKeyword(CardKeyword.Ethereal, UpgradeType.Remove);
        WithPower<DevaPower>(1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<DevaPower>(this);
    }
}