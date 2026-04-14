using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace Watcher.Code.Keywords;

public static class WatcherKeywords
{
    [CustomEnum] [KeywordProperties(AutoKeywordPosition.After)]
    public static CardKeyword Scry;

    public static bool IsScry(this CardModel card)
    {
        return card.Keywords.Contains(Scry);
    }
}