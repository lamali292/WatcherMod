using System.Reflection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace Watcher.Code.Tests;

/// Test-harness-only shims for game APIs whose signatures differ between game versions
/// (the mod is compiled once against beta and must run unchanged on public).
internal static class GameApiCompat
{
    /// RunState.CreateForTest gained `isCustomGame` in 0.113; bind by parameter name so either shape works.
    public static RunState CreateRunStateForTest(IReadOnlyList<Player> players, string seed)
    {
        var method = typeof(RunState).GetMethod("CreateForTest", BindingFlags.Public | BindingFlags.Static)
                     ?? throw new MissingMethodException("RunState.CreateForTest not found");
        var args = method.GetParameters()
            .Select(p => p.Name switch
            {
                "players" => players,
                "seed" => seed,
                _ => p.HasDefaultValue ? p.DefaultValue : null
            })
            .ToArray();
        return (RunState)method.Invoke(null, args)!;
    }

    /// CombatState's ctor replaced `multiplayerScalingModel` with `runModels` + `dominion` in 0.113.
    public static CombatState CreateCombatState(EncounterModel encounter, RunState run)
    {
        var ctor = typeof(CombatState).GetConstructors().First(c => c.GetParameters().Length > 0);
        var args = ctor.GetParameters().Select(p => p.Name switch
        {
            "encounter" => encounter,
            "runState" => run,
            "modifiers" => run.Modifiers,
            "badgeModels" => run.BadgeModels,
            "runModels" => GetRunProp(run, "RunModels"),
            "dominion" => GetRunProp(run, "Dominion"),
            "multiplayerScalingModel" => GetRunProp(run, "MultiplayerScalingModel"),
            _ => p.HasDefaultValue ? p.DefaultValue : null
        }).ToArray();
        return (CombatState)ctor.Invoke(args);
    }

    private static object? GetRunProp(RunState run, string name) =>
        typeof(RunState).GetProperty(name, BindingFlags.Public | BindingFlags.Instance)?.GetValue(run);

    /// EncounterModel.DebugRandomizeRng returns void on 0.107 and ulong on 0.113; the return value is unused.
    public static void RandomizeEncounterRng(EncounterModel encounter) =>
        typeof(EncounterModel).GetMethod("DebugRandomizeRng", BindingFlags.Public | BindingFlags.Instance)!
            .Invoke(encounter, null);
}
