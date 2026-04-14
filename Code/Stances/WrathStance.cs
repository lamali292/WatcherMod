using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Watcher.Code.Stances;

public sealed class WrathStance : StancePower
{
    private const string DamageMultiplier = "DamageMultiplier";
    protected override string AuraScenePath => "res://Watcher/scenes/watcher_mod/vfx/wrath_aura.tscn";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new(DamageMultiplier, 2m)];

    public override decimal ModifyDamageMultiplicative(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (dealer == Owner || target == Owner)
            return DynamicVars[DamageMultiplier].BaseValue;

        return 1m;
    }
}