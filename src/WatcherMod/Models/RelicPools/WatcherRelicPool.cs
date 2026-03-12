// Decompiled with JetBrains decompiler
// Type: MegaCrit.Sts2.Core.Models.RelicPools.DefectRelicPool
// Assembly: sts2, Version=0.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F46CB25A-BD02-40F0-A1D2-D82E621AB8D8
// Assembly location: D:\SteamLibrary\steamapps\common\Slay the Spire 2\data_sts2_windows_x86_64\sts2.dll

using Godot;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Unlocks;
using WatcherMod.Relics;

namespace WatcherMod.Models.RelicPools;

public sealed class WatcherRelicPool : RelicPoolModel
{
    public override string EnergyColorName => "watcher";

    public override Color LabOutlineColor => StsColors.purple;

    protected override IEnumerable<RelicModel> GenerateAllRelics()
    {
        return
        [
            ModelDb.Relic<PureWater>(),
            ModelDb.Relic<Damaru>(),
            ModelDb.Relic<Duality>(),
            ModelDb.Relic<TeardropLocket>(),
            ModelDb.Relic<GoldenEye>(),
            ModelDb.Relic<HolyWater>(),
            ModelDb.Relic<VioletLotus>(),
            ModelDb.Relic<Melange>()
        ];
    }

    public override IEnumerable<RelicModel> GetUnlockedRelics(UnlockState unlockState)
    {
        var list = AllRelics.ToList();
        return list;
    }
}