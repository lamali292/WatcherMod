# The Watcher — Slay the Spire 2

This mod ports **The Watcher** from Slay the Spire into Slay the Spire 2, recreating her full gameplay experience including cards, relics, and stances. It is one of the first character mods for Slay the Spire 2.

# Dependency
This mod requires BaseLib-StS2 for better mod compatibility and future update support.

# ⚠️ Important Notice (Read Before Using This Repo)

This project is **NOT A TEMPLATE** and is not intended to be used as a base for creating your own mods.

If you need a template use this https://github.com/Alchyr/ModTemplate-StS2/wiki/Setup

A large portion of this mod is tightly coupled to its internal structure, including (but not limited to):

- the mod ID
- folder structure
- animation and asset paths
- internal registration systems
- various hardcoded assumptions required for the Watcher implementation

Because of this, reusing this project directly (e.g. copying it and modifying it into your own mod) will very likely:

- break compatibility with this mods.
- cause ID conflicts
- introduce hard-to-debug issues
- create maintenance problems across updates

It would also be unfair to the modding ecosystem if multiple incompatible mods were built directly on top of this project.

## ✅ What is allowed
- Using this project as a reference
- Learning from the implementation
- Copying individual pieces of code or logic into your own properly structured mod
## ❌ What is not recommended
- Renaming this project and publishing it as your own mod base
- Using it as a plug-and-play template
- Extending it directly without fully understanding and restructuring it


## Features

- 82 Watcher cards + associated powers / status effects, including 5 multiplayer-only cards and the Ancient cards (Revelation is the Archaic Tooth upgrade of Vigilance)

- 11 related colorless cards (Miracle, Smite, Insight, Safety, ...)

- 8 relics and 3 potions unique to the Watcher

- Fully implemented **Wrath**, **Calm**, and **Divinity** stances

- Stance, Mantra and character sounds through FMOD (bank built from the `fmod/` project)



## Setup - for developers


### 1. Download the Repository



Clone the repository:



```bash
git clone https://github.com/lamali292/WatcherMod.git
```



### 2. Configure `local.props`



Copy `local.props.example` to `local.props` and fill in your paths:

```xml
<GodotPath>C:\...\MegaDot\MegaDot_v4.5.1_mono.exe</GodotPath>
<SteamLibraryPath>C:\Program Files (x86)\Steam\steamapps</SteamLibraryPath>
```



You only need to change:



- **SteamLibraryPath** → Path to your Steam Library

- **GodotPath** → Path to your Godot

(`local.props.example` also has optional entries for a second game install, beta + public.)
  
Recommended: [MegaDot — MegaCrit's custom Godot fork](https://megadot.megacrit.com/)

Standard: [Godot 4.5.1 .Net exe](https://godotengine.org/download/archive/4.5.1-stable/)



### 3. Generate the image atlases



Card, power, relic and potion art is **not** committed as atlases. The source PNGs live in `ImageGen/images/<kind>/` (named after the card/power/relic ID, e.g. `battle_hymn.png`) and are packed into `Watcher/images/` by the ImageGen tool. Run it once after cloning and again whenever art changes (CI and `release.ps1` do this automatically):

```bash
dotnet run --project ImageGen/ImageGen.csproj
```

Art that is missing falls back to a `todo` placeholder.



### 4. Build the Mod



Build the project using your IDE or the .NET CLI.  

After building, mod is placed into your Slay the Spire 2 mods folder.

```xml
...\Steam\steamapps\common\Slay the Spire 2\mods\Watcher
```



### Development Notes



Creating new characters in Slay the Spire 2 requires extensive patching, so there may still be undiscovered issues or crashes. Since this is a large port with many card interactions, I haven’t been able to test everything yet.



I’d greatly appreciate it if you report any problems as issues or contribute fixes via pull requests!



Animations are not included because Slay the Spire 2 uses Spine, and I currently don’t have the time or resources to learn it.

Audio: the sounds are FMOD events. The compiled banks are committed in `Watcher/audio/`; to change them open the project in `fmod/` with FMOD Studio (see `fmod/README.md`), build, and copy `Master.strings.bank` and `Watcher.bank` over.
