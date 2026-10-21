using Godot;
using FileAccess = Godot.FileAccess;

namespace Watcher.Code.Audio;

internal static class FmodServer
{
    private static readonly StringName Singleton = new("FmodServer");
    private static readonly StringName LoadBankMethod = new("load_bank");

    public static GodotObject? Get()
    {
        try
        {
            if (!Engine.HasSingleton(Singleton))
                return null;

            var server = Engine.GetSingleton(Singleton);
            return server is not null && GodotObject.IsInstanceValid(server) ? server : null;
        }
        catch (Exception ex)
        {
            WatcherMainFile.Logger.Error($"[Audio] FmodServer singleton: {ex.Message}");
            return null;
        }
    }

    public static bool Call(StringName method, params Variant[] args)
    {
        return Call(method, out _, args);
    }

    private static bool Call(StringName method, out Variant result, params Variant[] args)
    {
        result = default;
        if (Get() is not { } server || !server.HasMethod(method))
            return false;

        try
        {
            result = args.Length == 0 ? server.Call(method) : server.Call(method, args);
            return true;
        }
        catch (Exception ex)
        {
            WatcherMainFile.Logger.Error($"[Audio] FMOD {method}: {ex.Message}");
            return false;
        }
    }
    

    public static GodotObject? LoadBank(string resourcePath)
    {
        if (string.IsNullOrWhiteSpace(resourcePath) || !FileAccess.FileExists(resourcePath))
        {
            WatcherMainFile.Logger.Warn($"[Audio] load_bank: missing path: {resourcePath}");
            return null;
        }

        if (!Call(LoadBankMethod, out var result, resourcePath, 0))
        {
            WatcherMainFile.Logger.Warn($"[Audio] load_bank: call failed: {resourcePath}");
            return null;
        }

        switch (result.VariantType)
        {
            case Variant.Type.Bool when result.AsBool():
                return null;

            case Variant.Type.Object
                when result.AsGodotObject() is { } bank && GodotObject.IsInstanceValid(bank):
                return bank;

            default:
                WatcherMainFile.Logger.Warn($"[Audio] load_bank: not loaded ({result.VariantType}): {resourcePath}");
                return null;
        }
    }
}