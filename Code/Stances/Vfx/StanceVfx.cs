using Godot;
using MegaCrit.Sts2.Core.Assets;
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

    /// <summary>
    ///     Play a one-shot SFX from an OGG file.
    /// </summary>
    public static void PlayStanceSfx(string path)
    {
        if (NonInteractiveMode.IsActive) return;

        AudioStream stream;
        try
        {
            stream = PreloadManager.Cache.GetAsset<AudioStream>(path);
        }
        catch
        {
            GD.PrintErr($"[StanceVfx] Could not load audio: {path}");
            return;
        }

        var audioPlayer = new AudioStreamPlayer();
        audioPlayer.Stream = stream;
        audioPlayer.Bus = "SFX";
        audioPlayer.Finished += () => audioPlayer.QueueFree();

        var combatRoom = NCombatRoom.Instance;
        if (combatRoom != null)
        {
            combatRoom.AddChild(audioPlayer);
            audioPlayer.Play();
        }
        else
        {
            audioPlayer.QueueFree();
        }
    }
}