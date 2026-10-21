using Godot;

namespace Watcher.Code.Audio;

public static class FmodStudio
{
    private static readonly Lock Gate = new();
    private static readonly List<string> PendingBanks = [];

    // load_bank returns a Ref<FmodBank> (RefCounted). Without a managed reference held somewhere,
    // the .NET GC is free to finalize the wrapper, which drops the native refcount and unloads the
    // bank out from under the game — sounds start silently failing ("cannot find sfx path") once a
    // GC pass happens to run. This field's only job is to exist; it is never meant to be queried.
    // ReSharper disable once CollectionNeverQueried.Local
    private static readonly List<GodotObject> KeepAliveBankRefs = [];
    private static bool _ready;

    private static readonly StringName WaitForAllLoads = new("wait_for_all_loads");

    public static void RegisterBank(string resourcePath)
    {
        if (string.IsNullOrWhiteSpace(resourcePath))
            return;

        bool flushNow;
        lock (Gate)
        {
            PendingBanks.Add(resourcePath.Trim());
            flushNow = _ready;
        }

        if (flushNow)
            Flush();
    }

    public static void OnDeferredInitializationCompleted()
    {
        lock (Gate)
        {
            _ready = true;
        }

        Flush();
    }

    private static void Flush()
    {
        if (FmodServer.Get() is null)
            return;

        List<string> banks;
        lock (Gate)
        {
            if (PendingBanks.Count == 0)
                return;

            banks = [.. PendingBanks];
            PendingBanks.Clear();
        }

        foreach (var path in banks)
            if (FmodServer.LoadBank(path) is { } bank)
                lock (Gate)
                {
                    KeepAliveBankRefs.Add(bank);
                }

        if (banks.Count > 0)
            FmodServer.Call(WaitForAllLoads);

        WatcherMainFile.Logger.Info($"[Audio] FMOD flush: {banks.Count} bank(s).");
    }
}
