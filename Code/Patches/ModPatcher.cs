using System.Diagnostics;
using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;

namespace Watcher.Code.Patches;

public sealed class ModPatcher
{
    private readonly Harmony _harmony;
    private readonly Logger _logger;
    private readonly List<Type> _types = [];
    private bool _applied;

    /// Log every successfully patched method individually (verbose). Failures are always logged.
    public bool Verbose { get; set; }

    private ModPatcher(string modId, Logger logger)
    {
        _harmony = new Harmony(modId);
        _logger = logger;
    }

    public static ModPatcher Create(string modId, Logger logger) => new(modId, logger);

    public ModPatcher Add(Type patchClass)
    {
        _types.Add(patchClass);
        return this;
    }

    public ModPatcher AddAllFrom(Assembly assembly)
    {
        var found = SafeGetTypes(assembly)
            .Where(t => t.GetCustomAttributes(typeof(HarmonyPatch), inherit: false).Length > 0)
            .ToList();
        _types.AddRange(found);
        _logger.Info($"Discovered {found.Count} patch classes in {assembly.GetName().Name}.");
        return this;
    }

    public ModPatcher Exclude(Type patchClass)
    {
        _types.Remove(patchClass);
        return this;
    }

    public void PatchAll()
    {
        if (_applied)
        {
            _logger.Warn("PatchAll called twice; ignoring.");
            return;
        }
        _applied = true;

        var sw = Stopwatch.StartNew();
        var failures = new List<string>();
        var timings = new List<(string Name, long Ms)>();
        var okClasses = 0;
        var okMethods = 0;

        foreach (var type in _types.Distinct().OrderBy(t => t.FullName))
        {
            var clock = Stopwatch.StartNew();
            var result = Apply(type);
            clock.Stop();
            timings.Add((type.Name, clock.ElapsedMilliseconds));

            if (result.Error != null)
            {
                failures.Add($"{type.Name}: {result.Error}");
            }
            else
            {
                okClasses++;
                okMethods += result.MethodCount;
            }
        }

        sw.Stop();

        if (failures.Count == 0)
            _logger.Info($"Patching complete: {okClasses} classes, {okMethods} methods in {sw.ElapsedMilliseconds}ms.");
        else
            _logger.Error(
                $"Patching finished with {failures.Count} FAILURE(S) " +
                $"({okClasses} classes / {okMethods} methods OK, {sw.ElapsedMilliseconds}ms):\n  - " +
                string.Join("\n  - ", failures));

        var slowest = timings.OrderByDescending(t => t.Ms).Take(10);
        _logger.Info("Slowest patches:\n  " +
                     string.Join("\n  ", slowest.Select(t => $"{t.Ms,5}ms  {t.Name}")));
    }

    private (int MethodCount, string? Error) Apply(Type patchClass)
    {
        try
        {
            var patched = _harmony.CreateClassProcessor(patchClass).Patch();

            if (patched == null || patched.Count == 0)
                return (0, "patched ZERO methods (TargetMethod(s) returned null, or wrong signature?)");

            if (!Verbose) return (patched.Count, null);
            foreach (var method in patched)
                _logger.Info($"  {patchClass.Name} -> {Describe(method)}");

            return (patched.Count, null);
        }
        catch (Exception ex)
        {
            // First line of the exception is usually the useful part; full trace on demand
            var firstLine = ex.Message.Split('\n')[0].Trim();
            _logger.Error($"{patchClass.Name}: FAILED to apply.\n{ex}");
            return (0, firstLine);
        }
    }

    private static string Describe(MethodBase method)
        => $"{method.DeclaringType?.Name}.{method.Name}";

    private static Type[] SafeGetTypes(Assembly a)
    {
        try { return a.GetTypes(); }
        catch (ReflectionTypeLoadException e) { return e.Types.Where(t => t != null).ToArray()!; }
    }
}