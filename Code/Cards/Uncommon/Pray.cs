using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Watcher.Code.Cards.Token;
using Watcher.Code.Character;
using Watcher.Code.Extensions;
using Watcher.Code.Powers;

namespace Watcher.Code.Cards.Uncommon;

[Pool(typeof(WatcherCardPool))]
public sealed class Pray() : CustomCardModel(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<MantraPower>(),
        HoverTipFactory.FromCard<Insight>()
    ];


    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<MantraPower>(3m)
    ];

    public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<MantraPower>(
            Owner.Creature,
            DynamicVars["MantraPower"].IntValue,
            Owner.Creature,
            this
        );

        // Create Insight card
        if (CombatState == null) return;
        var insightCard = CombatState.CreateCard<Insight>(Owner);

        // Shuffle it into draw pile (Random position)
        CardCmd.PreviewCardPileAdd(
            await CardPileCmd.AddGeneratedCardToCombat(
                insightCard,
                PileType.Draw,
                true,
                CardPilePosition.Random
            )
        );
    }

    protected override void OnUpgrade()
    {
        DynamicVars["MantraPower"].UpgradeValueBy(1m);
    }
}