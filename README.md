# The Watcher — Slay the Spire 2 (Beta Branch) | v0.99

This mod ports **The Watcher** from Slay the Spire into Slay the Spire 2, recreating her full gameplay experience including cards, relics, and stances. It is one of the first character mods for Slay the Spire 2.

# Dependency
This mod requires BaseLib-StS2 for better mod compatibility and future update support.

# NexusMods Release
The mod will be uploaded to NexusMods once BaseLib is available on the main Steam branch, not just public beta. All future updates will be published here in the meantime.

## Features



- 83 Watcher Cards + associated powers / status effects

- 10 related colorless

- 8 relics unique to the Watcher

- Fully implemented **Wrath**, **Calm**, and **Divinity** stances



## Setup



### 1. Download the Repository



Clone the repository:



```bash
git clone https://github.com/lamali292/WatcherMod.git
```



### 2. Configure `local.props`



Open the `local.props` file and update the paths to match your system:



```xml

<Project>
  <PropertyGroup>
    <!-- Paths -->
    <STS2GamePath>C:\Steam\steamapps\common\Slay the Spire 2</STS2GamePath>
    <GodotExePath>C:\Path\To\Godot\Godot_v4.x_mono.exe</GodotExePath>
  </PropertyGroup>
</Project>
```



You only need to change:



- **STS2GamePath** → Path to your Slay the Spire 2 installation

- **GodotExePath** → Path to your [Godot 4.5.1 .Net exe](https://godotengine.org/download/archive/4.5.1-stable/)



### 3. Build the Mod



Build the project using your IDE or the .NET CLI.  

After building, mod is placed into your Slay the Spire 2 mods folder.

```xml
...\common\Slay the Spire 2\mods\WatcherMod

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
