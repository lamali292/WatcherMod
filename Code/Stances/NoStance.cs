using Watcher.Code.Vfx;

namespace Watcher.Code.Stances;

public class NoStance : WatcherStanceModel
{
    public override bool ShouldReceiveCombatHooks => false;
    protected override StanceVfxConfig VfxConfig => new();
}