using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.CardPools;
using Watcher.Code.Abstract;
using Watcher.Code.Stances;

namespace Watcher.Code.Cards.Token;

[Pool(typeof(TokenCardPool))]
public sealed class EnterCalm : WatcherCardModel
{
    public EnterCalm() : base(-1, CardType.Skill, CardRarity.Token, TargetType.None)
    {
        WithStanceTip<CalmStance>();
    }

    public override bool CanBeGeneratedInCombat => false;
}
