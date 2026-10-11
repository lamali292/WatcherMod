namespace Watcher.Code.Tests;

public static class Assert
{
    public static void IsTrue(bool condition, string message = "Assertion failed.")
    {
        if (!condition) throw new Exception($"[Assert] {message}");
    }

    public static void AreEqual<T>(T expected, T actual, string message = "")
    {
        if (!EqualityComparer<T>.Default.Equals(expected, actual))
            throw new Exception($"[Assert] Expected {expected} but got {actual}. {message}");
    }
}