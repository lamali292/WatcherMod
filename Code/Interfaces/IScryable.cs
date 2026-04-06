using MegaCrit.Sts2.Core.Entities.Players;

namespace Watcher.Code.Cards.CardModels;

public interface IScryable
{
    Task OnScryed(Player player, int amount)
    {
        return Task.CompletedTask;
    }
}