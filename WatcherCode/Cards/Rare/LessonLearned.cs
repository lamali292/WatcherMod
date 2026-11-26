using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using Watcher.Code.Abstract;
using Watcher.Code.Character;
using Watcher.Code.Compatibility;

namespace Watcher.Code.Cards.Rare;

[Pool(typeof(WatcherCardPool))]
public sealed class LessonLearned : WatcherCardModel
{
    public LessonLearned() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(10, 3);
        WithKeywords(CardKeyword.Exhaust);
        WithTip(StaticHoverTip.Fatal);
    }

    public override bool CanBeGeneratedInCombat => false;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        var shouldTriggerFatal = cardPlay.Target.Powers.All(p => p.ShouldOwnerDeathTriggerFatal());
        var attackCommand = await CommonActions.CardAttack(this, cardPlay).WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        if (!shouldTriggerFatal || !attackCommand.Results.Any(r => r.Any(g => g is { WasTargetKilled: true, Receiver.IsSecondaryEnemy: false })))
            return;
        var upgradableCards = PileType.Deck.GetPile(Owner).Cards.Where(c => c.IsUpgradable).ToList();
        if (upgradableCards.Count > 0)
        {
            await Cmd.Wait(0.5f);
            var cardModel = Owner.RunState.Rng.Niche.NextItem(upgradableCards);
            if (cardModel == null) return;
            // Not CardCmd.Upgrade: it does nothing once combat is ending, and a killing blow on the last enemy
            // ends the combat before this runs, which would silently drop the upgrade.
            Owner.RunState.CurrentMapPointHistoryEntry?.GetEntry(Owner.NetId).UpgradedCards.Add(cardModel.Id);
            cardModel.UpgradeInternalCompat();
            cardModel.FinalizeUpgradeInternal();
            if (LocalContext.IsMe(Owner))
                NRun.Instance?.GlobalUi.CardPreviewContainer.AddChildSafely(NCardSmithVfx.Create([
                    cardModel
                ])!);
        }
    }
}
