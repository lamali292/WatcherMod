using BaseLib.Abstracts;
using Godot;

namespace Watcher.Code.Character;

public class WatcherPotionPool : CustomPotionPoolModel
{
    public override string EnergyColorName => Watcher.CharacterId;
    public override Color LabOutlineColor => Watcher.Color;
}