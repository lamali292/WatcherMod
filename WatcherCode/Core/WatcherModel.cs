using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using Watcher.Code.Events;
using Watcher.Code.Nodes;
using Watcher.Code.Stances;
using Watcher.Code.Vfx;

namespace Watcher.Code.Core;

public class WatcherModel() : CustomSingletonModel(HookType.Combat)
{
    // Keyed on the combat state: a fresh PlayerCombatState is created for every combat, so the stance starts as
    // NoStance on its own and never leaks from one combat (or run) into the next.
    private static readonly SpireField<PlayerCombatState, WatcherStanceModel> ActiveStance =
        new(WatcherModelDb.WatcherStance<NoStance>);

    public override bool ShouldReceiveCombatHooks => true;


    public static WatcherStanceModel GetStanceModel(Player player)
    {
        return player.PlayerCombatState is { } state
            ? ActiveStance[state] ?? WatcherModelDb.WatcherStance<NoStance>()
            : WatcherModelDb.WatcherStance<NoStance>();
    }

    public static bool IsInStance<T>(Player player) where T : WatcherStanceModel
    {
        return GetStanceModel(player) is T;
    }

    public static async Task SetStance<T>(PlayerChoiceContext ctx, IEnumerable<Player> players, CardModel? source)
        where T : WatcherStanceModel
    {
        foreach (var player in players)
        {
            await SetStance(ctx, player, WatcherModelDb.WatcherStance<T>(), source);
        }
    }

    

    public static async Task SetStance<T>(PlayerChoiceContext ctx, Player player, CardModel? source)
        where T : WatcherStanceModel
    {
        await SetStance(ctx, player, WatcherModelDb.WatcherStance<T>(), source);
    }

    /// <summary>
    ///     Switches stance without <c>OnExitStance</c> / <c>OnEnterStance</c> and without the OnStanceChange hooks, so
    ///     nothing gameplay-relevant (block, energy, draws) triggers. The old stance's aura, tint and sound are still
    ///     torn down and the eyes updated. For resets where a regular stance change makes no sense: death, combat end.
    /// </summary>
    public static Task SetStanceSilently<T>(Player player) where T : WatcherStanceModel
    {
        return SetStance(null, player, WatcherModelDb.WatcherStance<T>(), null, true);
    }

    // ctx is only null when silent: the exit/enter/change hooks are the only users of it.
    private static async Task SetStance(PlayerChoiceContext? ctx, Player player, WatcherStanceModel newCanonical,
        CardModel? source, bool silent = false)
    {
        if (player.PlayerCombatState is not { } state) return;

        var current = ActiveStance[state];
        if (current?.GetType() == newCanonical.GetType()) return;

        if (silent) current?.CleanupWithoutExit(player);
        else if (current != null) await current.OnExitStance(ctx!, player, source);

        var mutable = newCanonical.ToMutable(player);
        ActiveStance[state] = mutable;
        if (!silent) await mutable.OnEnterStance(ctx!, player, source);

        var visuals = NCombatRoom.Instance?.GetCreatureNode(player.Creature)?.Visuals as WatcherNCreatureVisuals;
        visuals?.SetEyeStance(mutable switch
        {
            WrathStance => "Wrath",
            CalmStance => "Calm",
            DivinityStance => "Divinity",
            _ => "None"
        });

        if (!silent) await WatcherHook.OnStanceChange(ctx!, player, current!, mutable);
    }

    // The stance itself resets with the new PlayerCombatState; only sound loops can outlive a combat
    // (aborted or reloaded), so make sure none are left over.
    public override Task BeforeCombatStart()
    {
        StanceVfxController.StopAllAmbience();
        return Task.CompletedTask;
    }

    // Combat ends without exiting the stance, which used to leave its sound loop running into the next fight.
    public override Task AfterCombatEnd(CombatRoom room)
    {
        return ResetStances();
    }

    // A lost combat never fires AfterCombatEnd, so without this the stance sound would keep playing on the
    // death screen. A prevented death (revive) keeps the stance.
    public override Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, CardModel? source, bool wasRemovalPrevented,
        float deathAnimLength)
    {
        if (wasRemovalPrevented || creature.Player is not { } player) return Task.CompletedTask;

        return SetStanceSilently<NoStance>(player);
    }

    private static async Task ResetStances()
    {
        var state = CombatManager.Instance.DebugOnlyGetState();
        if (state != null)
            foreach (var player in state.Players)
                await SetStanceSilently<NoStance>(player);

        // Also catches loops whose stance object is already gone (e.g. aborted or reloaded combat).
        StanceVfxController.StopAllAmbience();
    }
}
