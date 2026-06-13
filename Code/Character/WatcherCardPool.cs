using BaseLib.Abstracts;
using Godot;

namespace Watcher.Code.Character;

public sealed class WatcherCardPool : CustomCardPoolModel
{
    public override string Title => Watcher.CharacterId;

    public override float H => 0.8f; //Hue; changes the color.
    public override float S => 0.5f; //Saturation
    public override float V => 1f; //Brightness

    public override Color DeckEntryCardColor => Watcher.Color;

    public override bool IsColorless => false;

    public override string? BigEnergyIconPath => "res://Watcher/images/ui/combat/watcher_energy_icon.png";
    public override string? TextEnergyIconPath => "res://Watcher/images/ui/combat/text_watcher_energy_icon.png";
}
