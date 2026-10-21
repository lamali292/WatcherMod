using System.Runtime.CompilerServices;
using ImageGen;

static string ProjectDir([CallerFilePath] string file = "")
{
    return Path.GetDirectoryName(file)!;
}

var scriptDir = ProjectDir();
var force = args.Contains("--repack");

new PackCards(scriptDir, force).Run();
new PackPowers(scriptDir, force).Run();
new PackRelics(scriptDir, force).Run();
new PackPotions(scriptDir, force).Run();
