using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using Watcher.Code.Abstract;
using Watcher.Code.Cards.CardModels;

namespace Watcher.Code.Cards.Token;

[Pool(typeof(TokenCardPool))]
public sealed class FameAndFortune : WatcherCardModel, IWishable
{
    public FameAndFortune() : base(-1, CardType.Skill, CardRarity.Token, TargetType.None)
    {
        WithVar("Gold", 25, 5);
        WithTip(typeof(StrengthPower));
    }

    public async Task OnWish(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        VfxCmd.PlayOnCreature(Owner.Creature, "vfx/vfx_coin_explosion_regular");
        await PlayerCmd.GainGold(DynamicVars.Gold.IntValue, Owner);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await OnWish(choiceContext, cardPlay);
    }
}