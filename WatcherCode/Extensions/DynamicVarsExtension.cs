using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Watcher.Code.DynamicVars;

namespace Watcher.Code.Extensions;

public static class DynamicVarsExtension
{
    extension(DynamicVarSet vars)
    {
        /// <summary>
        ///     The "HitCount" variable, whether it is the game's <c>HitCountVar</c> or our <c>WatcherHitCountVar</c>
        ///     (<c>DynamicVars.HitCount</c> only exists on newer game builds and casts to the game type).
        /// </summary>
        public DynamicVar HitCountCompat
            => vars[WatcherHitCountVar.DefaultName];
    }
}
