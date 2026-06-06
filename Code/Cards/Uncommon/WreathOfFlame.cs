using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using Watcher.Code.Abstract;
using Watcher.Code.Character;

namespace Watcher.Code.Cards.Uncommon;

[Pool(typeof(WatcherCardPool))]
public sealed class WreathOfFlame : WatcherCardModel
{
    public WreathOfFlame() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithPower<VigorPower>(5, 3, false);
    }


    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<VigorPower>(ctx, this);
    }
}