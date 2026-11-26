using System.Reflection;
using MegaCrit.Sts2.Core.Models;

namespace Watcher.Code.Compatibility;

/// <summary>
///     Small signature changes between game versions that need no more than a reflection call. The mod is compiled
///     once against beta but also runs on public, so a direct call to a changed signature would throw
///     <see cref="MissingMethodException" /> there.
/// </summary>
public static class GameApiCompat
{
    private static readonly MethodInfo UpgradeInternalMethod = typeof(CardModel).GetMethod("UpgradeInternal")!;
    private static readonly bool UpgradeInternalHasSilent = UpgradeInternalMethod.GetParameters().Length == 1;

    /// <summary>
    ///     `CardModel.UpgradeInternal()` on older builds, `UpgradeInternal(bool silent = false)` on newer ones.
    ///     Prefer `CardCmd.Upgrade` for real gameplay upgrades, except where it can't be used: it does nothing once
    ///     combat is ending. This is the raw no-hooks upgrade.
    /// </summary>
    public static void UpgradeInternalCompat(this CardModel card)
    {
        UpgradeInternalMethod.Invoke(card, UpgradeInternalHasSilent ? [false] : []);
    }
}
