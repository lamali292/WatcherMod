using Godot;

namespace Watcher.Code.Extensions;

//Mostly utilities to get asset paths.
//Missing art falls back to a "todo" placeholder (sources: ImageGen/images/**/todo*.png), so a card/power/relic/potion
//without art still loads instead of throwing.
public static class StringExtensions
{
    private static string ImgPath(string subfolder, string file)
    {
        return $"{WatcherMainFile.ModId}/images/{subfolder}/{file}";
    }

    private static string WithFallback(string subfolder, string file, string fallbackFile)
    {
        var primaryPath = ImgPath(subfolder, file);
        return ResourceLoader.Exists($"res://{primaryPath}") ? primaryPath : ImgPath(subfolder, fallbackFile);
    }

    public static string CardImagePath(this string path)
    {
        return WithFallback("atlases/card_atlas.sprites", path, "todo.tres");
    }

    public static string PowerImagePath(this string path)
    {
        return WithFallback("atlases/power_atlas.sprites", path, "todo_power.tres");
    }

    public static string BigPowerImagePath(this string path)
    {
        return WithFallback("powers", path, "todo_power.png");
    }

    public static string BigRelicImagePath(this string path)
    {
        return WithFallback("relics", path, "todo.png");
    }

    public static string TresRelicImagePath(this string path)
    {
        return WithFallback("atlases/relic_atlas.sprites", path, path.Contains("outline") ? "todo_outline.tres" : "todo.tres");
    }

    public static string PackedPotionImagePath(this string path)
    {
        return WithFallback("atlases/potion_atlas.sprites", path,
            path.Contains("outline") ? "todo_potion_outline.tres" : "todo_potion.tres");
    }
}
