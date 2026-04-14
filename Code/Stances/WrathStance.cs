using Godot;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;
using MegaCrit.Sts2.Core.ValueProps;

namespace Watcher.Code.Stances;

public sealed class WrathStance : StancePower
{
    private const string DamageMultiplier = "DamageMultiplier";

    protected override string AuraScenePath => "res://Watcher/scenes/watcher_mod/vfx/wrath_aura.tscn";
    protected override string EyeTexturePath => "res://Watcher/images/watcher_parts/eye_wrath.png";
    protected override string EnterSfxPath => "res://Watcher/audio/wrath_enter.ogg";
    protected override string AmbienceLoopPath => "res://Watcher/audio/wrath_loop.ogg";
    protected override Color? ScreenFlashColor => new Color(1f, 0.15f, 0.1f, 1f);
    protected override ShakeStrength ScreenShakeStrength => ShakeStrength.Medium;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new(DamageMultiplier, 2m)];

    public override decimal ModifyDamageMultiplicative(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if ((dealer == Owner || target == Owner) && !props.HasFlag(ValueProp.Unpowered))
            return DynamicVars[DamageMultiplier].BaseValue;

        return 1m;
    }
}
