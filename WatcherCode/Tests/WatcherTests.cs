using MegaCrit.Sts2.Core.Models;

namespace Watcher.Code.Tests;

public class WatcherTests
{
    [CardTest(typeof(Character.Watcher))]
    public IEnumerable<CardTestCase> PlayWatcherCards(CharacterModel character) => AllCardsTest.PlayAllCards(character);
}
