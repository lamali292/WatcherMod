using MegaCrit.Sts2.Core.Entities.Powers;
using Watcher.Code.Abstract;

namespace Watcher.Code.Powers;

public class MarkPower : WatcherPowerModel
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
}