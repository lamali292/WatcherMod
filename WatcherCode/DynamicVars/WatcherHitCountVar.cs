using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Watcher.Code.DynamicVars;

/// <summary>
///     A copy of the game's <c>HitCountVar</c> ("HitCount", {HitCount} in card text): the number of hits of an attack,
///     previewed through <c>Hook.ModifyAttackHitCount</c> (Thumbscrew, ...). Public game builds (0.111) don't ship
///     <c>HitCountVar</c>, so this version works everywhere. Read it with <c>DynamicVars.HitCountCompat</c> (the
///     game's <c>DynamicVars.HitCount</c> casts to the game type). Create it with
///     <c>WatcherCardModel.WithHitCount</c>.
/// </summary>
public class WatcherHitCountVar(string name, int hits) : DynamicVar(name, hits)
{
    public const string DefaultName = "HitCount";

    public WatcherHitCountVar(int hits) : this(DefaultName, hits)
    {
    }

    public override void UpdateCardPreview(CardModel card, CardPreviewMode previewMode, Creature? target,
        bool runGlobalHooks)
    {
        PreviewValue = Modify(card, IntValue, runGlobalHooks);
    }

    /// <summary>The hit count after hooks such as Thumbscrew's, as the attack itself will apply them.</summary>
    private static decimal Modify(CardModel card, int hits, bool runGlobalHooks)
    {
        // Non-positive counts are placeholders (e.g. Expunger's "X"), not real hits.
        if (hits <= 0 || !runGlobalHooks || card.CombatState == null) return hits;
        return Hook.ModifyAttackHitCount(card.CombatState, DamageCmd.Attack(0m).FromCard(card, null), hits);
    }
}
