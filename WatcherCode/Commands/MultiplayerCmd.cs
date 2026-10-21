using BaseLib.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Watcher.Code.Commands;

// Commands that make SEVERAL players make a choice during one card/power.
//
// A PlayerChoiceContext only supports one chooser: reusing it for a second player breaks (NullReferenceException in
// WaitForActionToResumeExecutingAfterPlayerChoice). Like the base game's Huddle Up, every player therefore gets their own
// BranchingPlayerChoiceContext, so each choice runs on that player's own action queue and all players choose at once.
public static class MultiplayerCmd
{
    /// <summary>
    ///     Runs <paramref name="action" /> for every player, each in its own branch. The context's owner goes last, because
    ///     their branch is the existing context and blocks until they are done (the others do not block).
    /// </summary>
    public static async Task ForEachPlayer(PlayerChoiceContext ctx, AbstractModel source, IEnumerable<Player> players,
        Func<PlayerChoiceContext, Player, Task> action)
    {
        var localId = LocalContext.NetId;
        if (localId == null) return;

        var order = players.OrderBy(player => player.NetId == ctx.OwnerId).ToList();
        foreach (var player in order)
        {
            var branch = new BranchingPlayerChoiceContext(source, localId.Value, GameActionType.Combat, ctx);
            await branch.AssignTaskAndWaitForPauseOrCompletion(action(branch, player));
        }
    }

    /// <summary>Every given player Scrys <paramref name="amount" /> at the same time.</summary>
    public static Task Scry(PlayerChoiceContext ctx, AbstractModel source, IEnumerable<Player> players, int amount)
    {
        return ForEachPlayer(ctx, source, players, (branch, player) => ScryCmd.Execute(branch, player, amount));
    }
}
