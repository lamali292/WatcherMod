using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Watcher.Code.Cards.CardModels;
using Watcher.Code.Character;
using Watcher.Code.Extensions;
using Watcher.Code.Keywords;

namespace Watcher.Code.Cards.Uncommon;

[Pool(typeof(WatcherCardPool))]
public sealed class Weave() : WatcherCardModel(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy), IScryable
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(4m, ValueProp.Move)];
    


    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(WatcherKeywords.Scry)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    /// <summary>
    ///     Called whenever the player scrys.
    ///     Returns this card from discard pile to hand if it's still there.
    /// </summary>
    public async Task OnScryed(Player player, int amount)
    {
        if (player != Owner)
            return;

        // Check if this card is in discard pile
        var discardPile = PileType.Discard.GetPile(player);
        if (discardPile.Cards.Contains(this)) await CardPileCmd.Add(this, PileType.Hand);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2);
    }
}