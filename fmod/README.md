# FMOD Studio project

Minimal FMOD Studio project for Slay the Spire 2 mods, taken from
https://github.com/BAKAOLC/STS2_FModProject_Minimal (it only contains the game's Master bank setup).
Godot ignores this folder (`.gdignore`); it is not packed into the mod.

## Rules
- Never edit/rename/replace the Master bank (its own GUID was changed on purpose, see Build) — mixer references break and the game goes silent.
- Create **one new bank named `Watcher`** and assign all custom events to it. The bank name decides the
  file name: `Watcher.bank` is what `WatcherMainFile.Initialize` registers (`res://Watcher/audio/Watcher.bank`).
- Use event paths unlikely to collide with other mods (e.g. `event:/sfx/watcher/...`) and route them under
  the matching mixer path (`Window -> Mixer Routing`).

## Build
1. Open `fmod.fspro` in FMOD Studio, `File -> Build` (output: `fmod/Build/desktop/`).
2. Copy `Master.strings.bank` and `Watcher.bank` to `Watcher/audio/` (`Master.bank` is not loaded).
3. Play events with `SfxCmd.Play(WatcherSfx.MantraGain)` (constants in `WatcherCode/Audio/WatcherSfx.cs`, the `event:/...` paths).

**Master bank GUID:** the upstream minimal project's Master bank has the game's own GUID, which makes
`Master.strings.bank` fail to load next to the game's (`FMOD_ERR_EVENT_ALREADY_LOADED`, error 70). Like Downfall,
this project's Master *bank* object has its own GUID (`{ce596aab-...}`), while the mixer groups keep the game's GUIDs,
so events still route into the game's `sfx` bus. Do not change either.

## Events (bank `Watcher`, folder `event:/sfx/characters/watcher-watcher/`, routed to the `sfx` mixer group)
One-shots: `calm_enter`, `wrath_enter`, `divinity_enter`, `mantra_gain`, `watcher-watcher_select`
(the last one is the character's default select sfx path).
Loops: `calm_loop`, `wrath_loop`, `divinity_loop` — a loop region over the whole sound, -6 dB, with an ADSR
(600 ms attack/release) on the event master volume. Start/stop with `SfxCmd.PlayLoop(path, false)` /
`SfxCmd.StopLoop(path)`; stopping uses the release stage as the fade-out.
Paths are the constants in `WatcherCode/Audio/WatcherSfx.cs`; sources are in `fmod/Assets/`.
