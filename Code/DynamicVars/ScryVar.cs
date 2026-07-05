using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Watcher.Code.Events;

namespace Watcher.Code.DynamicVars;

public class ScryVar(decimal baseValue) : DynamicVar("Scry", baseValue)
{
    public override void UpdateCardPreview(
        CardModel card,
        CardPreviewMode previewMode,
        Creature? target,
        bool runGlobalHooks)
    {
        var originalDamage1 = IntValue;
        if (runGlobalHooks)
            originalDamage1 = WatcherHook.ModifyScryAmount(card.Owner, originalDamage1, out _);
        PreviewValue = originalDamage1;
    }
}