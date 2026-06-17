using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Watcher.Code.Stances.Vfx;

/// <summary>
///     Shared VFX utilities for stance visual effects.
/// </summary>
public static class StanceVfx
{
    /// <summary>
    ///     Scale factor for all stance VFX positions and sizes.
    ///     Adjust this if the watcher character gets resized.
    /// </summary>
    public const float VfxScale = 0.9f;
}