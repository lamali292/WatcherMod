using BaseLib.Abstracts;
using BaseLib.Cards.Variables;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Watcher.Code.Core;
using Watcher.Code.DynamicVars;
using Watcher.Code.Extensions;
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
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.tres".CardImagePath();

    protected ConstructedCardModel WithPower<T>(int baseVal, int upgrade,
        bool showTooltip)
        where T : PowerModel
    {
        WithVar(new DynamicVar(typeof(T).Name, baseVal).WithUpgrade(upgrade));
        if (showTooltip)
            WithTips(e => [HoverTipFactory.FromPower<T>(e.DynamicVars.Power<T>().IntValue)]);
        return this;
    }

    /// <summary>
    ///     The attack's hit count ("HitCount"): the previewed value includes hit-count modifiers such as Thumbscrew.
    ///     Pass <paramref name="name"/> to use a different variable name than "HitCount".
    /// </summary>
    protected ConstructedCardModel WithHitCount(int baseVal, int upgradeVal = 0,
        string name = WatcherHitCountVar.DefaultName)
    {
        return WithVar(new WatcherHitCountVar(name, baseVal).WithUpgrade(upgradeVal));
    }

    protected ConstructedCardModel WithScry(int baseVal, int upgrade)
    {
        WithVar(new ScryVar(baseVal).WithUpgrade(upgrade));
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