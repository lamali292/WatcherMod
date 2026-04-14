using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using Watcher.Code.Abstract;

namespace Watcher.Code.Cards.Token;

[Pool(typeof(TokenCardPool))]
public sealed class FameAndFortune : WishableWatcherCard
{
    public FameAndFortune() : base(-1, CardType.Skill, CardRarity.Token, TargetType.None)
    {
        WithVars(new GoldVar(25).WithUpgrade(5));
        WithTip(typeof(StrengthPower));
    }

    public override async Task OnWish(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        VfxCmd.PlayOnCreature(Owner.Creature, "vfx/vfx_coin_explosion_regular");
        await PlayerCmd.GainGold(DynamicVars.Gold.IntValue, Owner);
    }
    
}