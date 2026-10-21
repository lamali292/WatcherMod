using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.CardPools;
using Watcher.Code.Abstract;
using Watcher.Code.Stances;

namespace Watcher.Code.Cards.Token;

[Pool(typeof(TokenCardPool))]
public sealed class EnterWrath : WatcherCardModel
{
    public EnterWrath() : base(-1, CardType.Skill, CardRarity.Token, TargetType.None)
    {
        WithStanceTip<WrathStance>();
    }

    public override bool CanBeGeneratedInCombat => false;
}
