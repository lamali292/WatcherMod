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

namespace Watcher.Code.Cards.Token;

[Pool(typeof(TokenCardPool))]
public sealed class FameAndFortune() : WishModel(-1, CardType.Skill, CardRarity.Token, TargetType.None)
{
    public override CardPoolModel Pool => ModelDb.CardPool<TokenCardPool>();
    protected override IEnumerable<DynamicVar> CanonicalVars => [new GoldVar(25)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>()
    ];

    public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();

    public override async Task OnWish(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        VfxCmd.PlayOnCreature(Owner.Creature, "vfx/vfx_coin_explosion_regular");
        await PlayerCmd.GainGold(DynamicVars.Gold.IntValue, Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Gold.UpgradeValueBy(5);
    }
}