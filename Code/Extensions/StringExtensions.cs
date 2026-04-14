namespace Watcher.Code.Extensions;

//Mostly utilities to get asset paths.
public static class StringExtensions
{
    public static string ImagePath(this string path)
    {
        return Path.Join(WatcherMainFile.ModId, "images", path);
    }

    public static string CardImagePath(this string path)
    {
        return Path.Join(WatcherMainFile.ModId, "images", "card_portraits", path);
    }

    public static string BigCardImagePath(this string path)
    {
        return Path.Join(WatcherMainFile.ModId, "images", "card_portraits", "big", path);
    }

    public static string PowerImagePath(this string path)
    {
        return Path.Join(WatcherMainFile.ModId, "images", "powers", path);
    }

    public static string BigPowerImagePath(this string path)
    {
        return Path.Join(WatcherMainFile.ModId, "images", "powers", "big", path);
    }


    public static string RelicImagePath(this string path)
    {
        return Path.Join(WatcherMainFile.ModId, "images", "relics", path);
    }

    public static string BigRelicImagePath(this string path)
    {
        return Path.Join(WatcherMainFile.ModId, "images", "relics", "big", path);
    }

    public static string TresRelicImagePath(this string path)
    {
        return Path.Join(WatcherMainFile.ModId, "images", "relics", "tres", path);
    }

    public static string PackedPotionImagePath(this string path)
    {
        return Path.Join(WatcherMainFile.ModId, "images", "potions", "tres", path);
    }


    public static string CharacterUiPath(this string path)
    {
        return Path.Join(WatcherMainFile.ModId, "images", "charui", path);
    }
}