using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using Watcher.Code.Events;
using Watcher.Code.Nodes;
using Watcher.Code.Stances;

namespace Watcher.Code.Core;

public class WatcherModel() : CustomSingletonModel(true, false)
{
    private static readonly SpireField<Player, WatcherStanceModel> ActiveStance =
        new(WatcherModelDb.WatcherStance<NoStance>);
    
    
    public static WatcherStanceModel GetStanceModel(Player player)
    {
        return ActiveStance[player] ?? WatcherModelDb.WatcherStance<NoStance>();
    }

    public static bool IsInStance<T>(Player player) where T : WatcherStanceModel
    {
        return ActiveStance[player] is T;
    }

    
    public static async Task SetStance<T>(PlayerChoiceContext ctx, Player player, CardModel? source) where T : WatcherStanceModel
    {
        await SetStance(ctx, player, WatcherModelDb.WatcherStance<T>(), source);
    }

    private static async Task SetStance(PlayerChoiceContext ctx, Player player, WatcherStanceModel newCanonical, CardModel? source)
    {
        var current = ActiveStance[player];
        if (current?.GetType() == newCanonical.GetType()) return;

        if (current != null)
            await current.OnExitStance(ctx, player, source);

        var mutable = newCanonical.ToMutable(player);
        ActiveStance[player] = mutable;
        await mutable.OnEnterStance(ctx, player, source);

        var creatureNode = NCombatRoom.Instance?.GetCreatureNode(player.Creature);
        var visuals = creatureNode?.Visuals as WatcherNCreatureVisuals;
        visuals?.SetEyeStance(mutable switch
        {
            WrathStance => "wrath",
            CalmStance => "calm",
            DivinityStance => "divinity",
            _ => "RESET"
        });
        await WatcherHook.OnStanceChange(ctx, player, current!, ActiveStance[player]!);
    }
    
    public override async Task BeforeCombatStart()
    {
        var state = CombatManager.Instance.DebugOnlyGetState();
        if (state == null) return;
        foreach (var player in state.Players)
            ActiveStance[player] = WatcherModelDb.WatcherStance<NoStance>();
    }
    public override bool ShouldReceiveCombatHooks => true;
}