using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Watcher.Code.Core;
using Watcher.Code.DynamicVars;
using Watcher.Code.Extensions;
using Watcher.Code.Keywords;
using Watcher.Code.Stances;

namespace Watcher.Code.Abstract;

public abstract class WatcherCardModel(
    int canonicalEnergyCost,
    CardType type,
    CardRarity rarity,
    TargetType targetType,
    bool shouldShowInCardLibrary = true)
    : ConstructedCardModel(canonicalEnergyCost, type, rarity, targetType, shouldShowInCardLibrary)
{
    public sealed override string CustomPortraitPath =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();

    protected ConstructedCardModel WithPower<T>(int baseVal, int upgrade,
        bool showTooltip)
        where T : PowerModel
    {
        WithVar(new DynamicVar(typeof(T).Name, baseVal).WithUpgrade(upgrade));
        if (showTooltip)
            WithTips(e => [HoverTipFactory.FromPower<T>(e.DynamicVars.Power<T>().IntValue)]);
        return this;
    }

    protected ConstructedCardModel WithScry(int baseVal, int upgrade,
        bool showTooltip = true)
    {
        WithVar(new ScryVar(baseVal).WithUpgrade(upgrade));
        if (showTooltip)
            WithTip(WatcherKeywords.Scry);
        return this;
    }
    
    protected ConstructedCardModel WithPower<T>( int baseVal, bool showTooltip)
        where T : PowerModel
    {
        return WithPower<T>(baseVal, 0, showTooltip);
    }

    protected WatcherCardModel WithStanceTip<T>() where T : WatcherStanceModel
    {
        WithTip(new TooltipSource(_ => WatcherHoverTipFactory.FromStance<T>()));
        return this;
    }
}