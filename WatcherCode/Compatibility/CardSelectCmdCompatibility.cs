using System.Linq.Expressions;
using System.Reflection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.TestSupport;

namespace Watcher.Code.Compatibility;

public static class CardSelectCmdCompatibility
{
    private static readonly UseSelectorDel UseSelectorImpl = BuildUseSelector();

    /// <summary>
    ///     Installs a test-only ICardSelector. Newer game versions dropped the
    ///     `localOnly` parameter; on those, it's simply ignored.
    /// </summary>
    public static IDisposable UseSelector(ICardSelector selector, bool localOnly = false) =>
        UseSelectorImpl(selector, localOnly);

    private static UseSelectorDel BuildUseSelector()
    {
        var selector = Expression.Parameter(typeof(ICardSelector), "selector");
        var localOnly = Expression.Parameter(typeof(bool), "localOnly");

        var twoArgMethod = typeof(CardSelectCmd).GetMethod("UseSelector",
            BindingFlags.Public | BindingFlags.Static, null, [typeof(ICardSelector), typeof(bool)], null);
        if (twoArgMethod != null)
        {
            var call = Expression.Call(twoArgMethod, selector, localOnly);
            return Expression.Lambda<UseSelectorDel>(call, selector, localOnly).Compile();
        }

        var oneArgMethod = typeof(CardSelectCmd).GetMethod("UseSelector",
                               BindingFlags.Public | BindingFlags.Static, null, [typeof(ICardSelector)], null)
                           ?? throw new MissingMethodException("CardSelectCmd.UseSelector not found");
        var oneArgCall = Expression.Call(oneArgMethod, selector);
        var oneArgDel = Expression.Lambda<OneArgUseSelectorDel>(oneArgCall, selector).Compile();
        return (s, _) => oneArgDel(s);
    }

    private delegate IDisposable UseSelectorDel(ICardSelector selector, bool localOnly);

    private delegate IDisposable OneArgUseSelectorDel(ICardSelector selector);
}
