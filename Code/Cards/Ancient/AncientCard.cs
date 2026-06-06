using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Watcher.Code.Abstract;
using Watcher.Code.Character;
using Watcher.Code.Powers;
using Watcher.Code.Stances;

namespace Watcher.Code.Cards.Ancient;

[Pool(typeof(WatcherCardPool))]
public sealed class AncientCard : WatcherCardModel
{
    public AncientCard() : base(2, CardType.Power, CardRarity.Ancient, TargetType.None)
    {
        WithKeyword(CardKeyword.Ethereal, UpgradeType.Remove);
        WithPower<AncientCardPower>(50, false);
        WithStanceTip<WrathStance>();
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<AncientCardPower>(ctx, this);
    }
}