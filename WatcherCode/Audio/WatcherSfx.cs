namespace Watcher.Code.Audio;

// FMOD events of the Watcher bank (built from fmod/, see fmod/README.md), played through the base game's SfxCmd.
public static class WatcherSfx
{
    private const string Base = "event:/sfx/characters/watcher-watcher/";

    public const string CharacterSelect = Base + "watcher-watcher_select";
    public const string CalmEnter = Base + "calm_enter";
    public const string CalmLoop = Base + "calm_loop";
    public const string WrathEnter = Base + "wrath_enter";
    public const string WrathLoop = Base + "wrath_loop";
    public const string DivinityEnter = Base + "divinity_enter";
    public const string DivinityLoop = Base + "divinity_loop";
    public const string MantraGain = Base + "mantra_gain";
}
