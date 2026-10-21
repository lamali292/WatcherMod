namespace Watcher.Code.Extensions;

//Mostly utilities to get asset paths.
public static class StringExtensions
{
    public static string CardImagePath(this string path)
    {
        return Path.Join(WatcherMainFile.ModId, "images", "atlases", "card_atlas.sprites", path);
    }

    public static string PowerImagePath(this string path)
    {
        return Path.Join(WatcherMainFile.ModId, "images", "atlases", "power_atlas.sprites", path);
    }

    public static string BigPowerImagePath(this string path)
    {
        return Path.Join(WatcherMainFile.ModId, "images", "powers", path);
    }

    public static string BigRelicImagePath(this string path)
    {
        return Path.Join(WatcherMainFile.ModId, "images", "relics", path);
    }

    public static string TresRelicImagePath(this string path)
    {
        return Path.Join(WatcherMainFile.ModId, "images", "atlases", "relic_atlas.sprites", path);
    }

    public static string PackedPotionImagePath(this string path)
    {
        return Path.Join(WatcherMainFile.ModId, "images", "atlases", "potion_atlas.sprites", path);
    }
}
