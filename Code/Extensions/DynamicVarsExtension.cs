using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Watcher.Code.DynamicVars;

namespace Watcher.Code.Extensions;

public static class DynamicVarsExtension
{
    public static ScryVar Scry(this DynamicVarSet vard)
    {
        return (ScryVar)vard._vars[nameof(Scry)];
    }
}