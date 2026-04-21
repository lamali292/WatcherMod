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

- 83 Watcher Cards + associated powers / status effects

- 10 related colorless

- 8 relics unique to the Watcher

- Fully implemented **Wrath**, **Calm**, and **Divinity** stances



## Setup - for developers


### 1. Download the Repository



Clone the repository:



```bash
git clone https://github.com/lamali292/WatcherMod.git
```



### 2. Configure `local.props`



Open the `Watcher.csproj` file and update the paths to match your system:

i.e in Windows look for

```xml
<PropertyGroup Condition="'$(IsWindows)' == 'true'">
  <GodotPath Condition="'$(GodotPath)' == ''">C:\Path\To\Godot\Godot4.5.1.exe</GodotPath>
  <SteamLibraryPath Condition="'$(SteamLibraryPath)' == ''">C:\Program Files (x86)\Steam\steamapps</SteamLibraryPath>
  <!-- The below should not need to be changed. -->
  ...
</PropertyGroup>
```



You only need to change:



- **SteamLibraryPath** → Path to your Steam Library

- **GodotPath** → Path to your Godot
  
Recommended: [MegaDot — MegaCrit's custom Godot fork](https://megadot.megacrit.com/)

Standard: [Godot 4.5.1 .Net exe](https://godotengine.org/download/archive/4.5.1-stable/)



### 3. Build the Mod



Build the project using your IDE or the .NET CLI.  

After building, mod is placed into your Slay the Spire 2 mods folder.

```xml
...\Steam\steamapps\common\Slay the Spire 2\mods\Watcher
```



### Development Notes



Creating new characters in Slay the Spire 2 requires extensive patching, so there may still be undiscovered issues or crashes. Since this is a large port with many card interactions, I haven’t been able to test everything yet.



I’d greatly appreciate it if you report any problems as issues or contribute fixes via pull requests!



Animations are not included because Slay the Spire 2 uses Spine, and I currently don’t have the time or resources to learn it.


### Credits

chaendizzle - Calm, Divinity and Wrath SFX / VFX

Snumodder - Korean localization. 

NoFires - Chinese localization

Cany0udance - for helping me to port the animations. 
