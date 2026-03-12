# The Watcher - Slay the Spire II



The mod ports **The Watcher** from **Slay the Spire** into **Slay the Spire 2**.



It is (one of?) the **first character mod for Slay the Spire 2**, recreating the Watcher's gameplay systems including cards, relics, and stances.



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
    <STS2GamePath>C:\\Steam\\steamapps\\common\\Slay the Spire 2</STS2GamePath>
    <GodotExePath>C:\\Path\\To\\Godot\\Godot\_v4.x\_mono.exe</GodotExePath>
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

Snumodder - Korean localization. 
