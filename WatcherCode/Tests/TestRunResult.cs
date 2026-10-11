using System.Text.Json;
using System.Text.Json.Serialization;

namespace Watcher.Code.Tests;

public sealed record TestFailure(string Name, string Message, string Details);

public sealed record TestRunResult(
	string Seed,
	TimeSpan Duration,
	IReadOnlyList<string> Passed,
	IReadOnlyList<TestFailure> Failed)
{
	[JsonIgnore] public bool Success => Failed.Count == 0;

	public void WriteJson(string path)
	{
		var json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
		Directory.CreateDirectory(Path.GetDirectoryName(path)!);
		File.WriteAllText(path, json);
	}
}
