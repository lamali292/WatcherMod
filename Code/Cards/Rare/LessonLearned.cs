using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using Watcher.Code.Character;
using Watcher.Code.Extensions;

namespace Watcher.Code.Cards.Rare;

[Pool(typeof(WatcherCardPool))]
public sealed class LessonLearned() : CustomCardModel(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    // Base damage
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(10m, ValueProp.Move)
    ];

    public override HashSet<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(StaticHoverTip.Fatal)
    ];

    public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();


    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var card = this;
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var shouldTriggerFatal = cardPlay.Target.Powers.All(p => p.ShouldOwnerDeathTriggerFatal());
        var attackCommand = await DamageCmd.Attack(card.DynamicVars.Damage.BaseValue).FromCard(card)
            .Targeting(cardPlay.Target).WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
        if (!shouldTriggerFatal || !attackCommand.Results.Any(r => r.WasTargetKilled))
            return;
        var upgradableCards = PileType.Deck.GetPile(card.Owner).Cards.Where(c => c.IsUpgradable).ToList();
        if (upgradableCards.Count > 0)
        {
            await Cmd.Wait(0.5f);
            var cardModel = card.Owner.RunState.Rng.Niche.NextItem(upgradableCards);
            if (cardModel == null) return;
            card.Owner.RunState.CurrentMapPointHistoryEntry?.GetEntry(card.Owner.NetId).UpgradedCards.Add(cardModel.Id);
            cardModel.UpgradeInternal();
            cardModel.FinalizeUpgradeInternal();
            NRun.Instance?.GlobalUi.CardPreviewContainer.AddChildSafely(NCardSmithVfx.Create([
                cardModel
            ])!);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m); // 10 -> 13
    }
}