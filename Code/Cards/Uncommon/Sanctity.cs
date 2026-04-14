using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Watcher.Code.Character;
using Watcher.Code.Extensions;

namespace Watcher.Code.Cards.Uncommon;

[Pool(typeof(WatcherCardPool))]
public sealed class Sanctity() : CustomCardModel(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(6m, ValueProp.Move)
    ];

    // Optional: Glow gold wenn letzte Karte ein Skill war
    protected override bool ShouldGlowGoldInternal => WasLastCardPlayedSkill;

    private bool WasLastCardPlayedSkill
    {
        get
        {
            var lastCardEntry = CombatManager.Instance.History.CardPlaysStarted
                .LastOrDefault(e =>
                    e.CardPlay.Card.Owner == Owner &&
                    e.HappenedThisTurn(CombatState) &&
                    e.CardPlay.Card != this);

            if (lastCardEntry == null) return false;

            return lastCardEntry.CardPlay.Card.Type == CardType.Skill;
        }
    }

    public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);

        // Check if last card was Skill
        if (WasLastCardPlayedSkill) await CardPileCmd.Draw(choiceContext, 2, Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
    }
}