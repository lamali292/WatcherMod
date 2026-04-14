using Godot;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;

namespace Watcher.Code.Vfx;

public record StanceVfxConfig(
    string? AuraScenePath = null,
    Color? BodyTint = null,
    string? EnterSfxPath = null,
    Color? ScreenFlashColor = null,
    ShakeStrength ScreenShakeStrength = ShakeStrength.None,
    string? AmbienceLoopPath = null
)
{
    public IEnumerable<string> AssetPaths
    {
        get
        {
            if (AuraScenePath != null) yield return AuraScenePath;
            if (AmbienceLoopPath != null) yield return AmbienceLoopPath;
            if (EnterSfxPath != null) yield return EnterSfxPath;
        }
    }
}