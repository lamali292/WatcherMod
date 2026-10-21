using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Watcher.Code.Core;
using Watcher.Code.Stances;

namespace Watcher.Code.Commands;

public static class StanceCmd
{
    public static Task EnterWrath(PlayerChoiceContext ctx, Player player, CardModel? cardSource)
    {
        return WatcherModel.SetStance<WrathStance>(ctx, player, cardSource);
    }

    public static Task EnterCalm(PlayerChoiceContext ctx, Player player, CardModel? cardSource)
    {
        return WatcherModel.SetStance<CalmStance>(ctx, player, cardSource);
    }

    public static Task EnterDivinity(PlayerChoiceContext ctx, Player player, CardModel? cardSource)
    {
        return WatcherModel.SetStance<DivinityStance>(ctx, player, cardSource);
    }

    public static Task ExitStance(PlayerChoiceContext ctx, Player player, CardModel? cardSource)
    {
        return WatcherModel.SetStance<NoStance>(ctx, player, cardSource);
    }
    
    public static Task EnterWrath(PlayerChoiceContext ctx, IEnumerable<Player> players, CardModel? cardSource)
    {
        return WatcherModel.SetStance<WrathStance>(ctx, players, cardSource);
    }

    public static Task EnterCalm(PlayerChoiceContext ctx, IEnumerable<Player> players, CardModel? cardSource)
    {
        return WatcherModel.SetStance<CalmStance>(ctx, players, cardSource);
    }

    public static Task EnterDivinity(PlayerChoiceContext ctx, IEnumerable<Player> players, CardModel? cardSource)
    {
        return WatcherModel.SetStance<DivinityStance>(ctx, players, cardSource);
    }

    public static Task ExitStance(PlayerChoiceContext ctx, IEnumerable<Player> players, CardModel? cardSource)
    {
        return WatcherModel.SetStance<NoStance>(ctx, players, cardSource);
    }
}