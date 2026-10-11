namespace Watcher.Code.Tests;

[AttributeUsage(AttributeTargets.Method)]
public class CardTestAttribute(Type? characterType = null, Type? encounterType = null, int playerCount = 1) : Attribute
{
    public Type? CharacterType { get; } = characterType;
    public Type? EncounterType { get; } = encounterType;

    /// Number of players in the combat (all playing <see cref="CharacterType"/>). Player 1 is the local one.
    public int PlayerCount { get; } = playerCount;
}
