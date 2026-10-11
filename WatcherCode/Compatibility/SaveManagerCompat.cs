using System.Reflection;
using MegaCrit.Sts2.Core.Saves;

namespace Watcher.Code.Compatibility;

public static class SaveManagerCompat
{
    private static readonly PropertyInfo? IsPrefsLoadedProperty =
        typeof(SaveManager).GetProperty("IsPrefsLoaded", BindingFlags.Public | BindingFlags.Instance);

    /// <summary>
    /// Version-safe replacement for SaveManager.IsPrefsLoaded, which older builds (V107) don't have.
    /// There the only signal is PrefsSave itself: it stays null until InitPrefsData runs,
    /// despite its non-nullable annotation.
    /// </summary>
    public static bool ArePrefsLoaded(this SaveManager saveManager) =>
        IsPrefsLoadedProperty != null
            ? (bool)IsPrefsLoadedProperty.GetValue(saveManager)!
            : (object?)saveManager.PrefsSave != null;
}
