using BaseLib.Cards.Variables;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Watcher.Code.Extensions;

public static class DynamicVarsExtension
{
    public static ScryVar Scry(this DynamicVarSet vard)
    {
        return (ScryVar)vard._vars[nameof(Scry)];
    }
}