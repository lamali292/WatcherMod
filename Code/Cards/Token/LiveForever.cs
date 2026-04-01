using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using Watcher.Code.Cards.CardModels;
using Watcher.Code.Extensions;
using Watcher.Code.Powers;

namespace Watcher.Code.Cards.Token;

[Pool(typeof(TokenCardPool))]
public sealed class LiveForever() : WatcherCardModel(-1, CardType.Power, CardRarity.Token, TargetType.None), IWishable
{
    public override CardPoolModel Pool => ModelDb.CardPool<TokenCardPool>();
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<PlatedArmorPower>(6)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<PlatedArmorPower>()
    ];

    

    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await OnWish(choiceContext, cardPlay);
    }
    
    public async Task OnWish(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<PlatedArmorPower>(Owner.Creature, DynamicVars["PlatedArmorPower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["PlatedArmorPower"].UpgradeValueBy(2);
    }
}