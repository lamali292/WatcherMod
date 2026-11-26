using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Watcher.Code.Abstract;
using Watcher.Code.Character;
using Watcher.Code.Commands;
using Watcher.Code.Compatibility;
using Watcher.Code.Extensions;
using Watcher.Code.Stances;

namespace Watcher.Code.Cards.Uncommon;

[Pool(typeof(WatcherCardPool))]
public sealed class Tantrum : WatcherCardModel
{
    public Tantrum() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(3);
        WithHitCount(3, 1);
        WithStanceTip<WrathStance>();
    }


    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).WithHitCount(DynamicVars.HitCountCompat.IntValue).Execute(ctx);
        await StanceCmd.EnterWrath(ctx, Owner, cardPlay.Card);
    }

    // Shuffle into the draw pile instead of going to the discard pile after being played, like the base game's
    // Shining Strike (top of the draw pile) overrides it.
    protected override CardLocation GetResultLocationForCardPlay()
    {
        var location = base.GetResultLocationForCardPlay();
        if (location.pileType != PileType.Discard) return location;
        location.pileType = PileType.Draw;
        location.position = CardPilePosition.Random;
        return location;
    }
}
