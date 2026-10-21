using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using Watcher.Code.Core;
using Watcher.Code.Stances;

namespace Watcher.Code.Extensions;

public static class PlayerExtensions
{
    extension(Player player)
    {
        
        public bool IsInWatcherStance<T>()
            where T : WatcherStanceModel
        {
            return WatcherModel.IsInStance<T>(player);
        }

        
        public WatcherStanceModel WatcherStance => WatcherModel.GetStanceModel(player);
        
        public IReadOnlyList<Player> AllTeammates
            => player.Creature.CombatState!.GetTeammatesOf(player.Creature)
                .Where(e => e.IsHittable)
                .Select(c => c.Player)
                .OfType<Player>().ToArray();
            
        public IReadOnlyList<Creature> AllOpponents
            => player.Creature.CombatState!.GetOpponentsOf(player.Creature)
                .Where(e => e.IsHittable)
                .ToArray();

        public IReadOnlyList<Player> OtherTeammates => player.AllTeammates.Where(p => p != player).ToArray();
        public Player? RandomOtherTeammate => player.RunState.Rng.CombatTargets.NextItem(player.OtherTeammates);

        public IReadOnlyList<CardModel> DeckPile => PileType.Deck.GetPile(player).Cards;
        public IReadOnlyList<CardModel> Hand => PileType.Hand.GetPile(player).Cards;
        public IReadOnlyList<CardModel> DiscardPile => PileType.Discard.GetPile(player).Cards;
        public IReadOnlyList<CardModel> DrawPile => PileType.Draw.GetPile(player).Cards;
        public IReadOnlyList<CardModel> ExhaustPile => PileType.Exhaust.GetPile(player).Cards;

        public IEnumerable<CardModel> GetAllCombatCards => player.PlayerCombatState?.AllCards ?? [];
    }
}