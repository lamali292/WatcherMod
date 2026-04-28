using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using Watcher.Code.Abstract;
using Watcher.Code.Character;

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


    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var shouldTriggerFatal = cardPlay.Target.Powers.All(p => p.ShouldOwnerDeathTriggerFatal());
        var attackCommand = await CommonActions.CardAttack(this, cardPlay).WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        if (!shouldTriggerFatal || !attackCommand.Results.Any(r => r.WasTargetKilled))
            return;
        var upgradableCards = PileType.Deck.GetPile(Owner).Cards.Where(c => c.IsUpgradable).ToList();
        if (upgradableCards.Count > 0)
        {
            await Cmd.Wait(0.5f);
            var cardModel = Owner.RunState.Rng.Niche.NextItem(upgradableCards);
            if (cardModel == null) return;
            Owner.RunState.CurrentMapPointHistoryEntry?.GetEntry(Owner.NetId).UpgradedCards.Add(cardModel.Id);
            cardModel.UpgradeInternal();
            cardModel.FinalizeUpgradeInternal();
            if (LocalContext.IsMe(Owner))
                NRun.Instance?.GlobalUi.CardPreviewContainer.AddChildSafely(NCardSmithVfx.Create([
                    cardModel
                ])!);
        }
    }
}