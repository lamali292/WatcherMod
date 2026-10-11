namespace Watcher.Code.Tests;

public sealed record CardTestCase(string Name, Func<TestContext, Task> Run);