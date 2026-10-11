using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.TestSupport;

namespace Watcher.WatcherCode.Vfx;

/*
[GlobalClass]
public partial class NDevaFormVfx  : NFormVfx
{
    private static readonly string ScenePath = "res://Watcher/scenes/watcher/vfx_deva_form_idle_vfx.tscn";

    
    public static NDevaFormVfx? Create(Creature target)
    {
        if (TestMode.IsOn)
            return null;
        var creatureNode = NCombatRoom.Instance?.GetCreatureNode(target);
        if (creatureNode == null) return null;
        var formVfx = PreloadManager.Cache.GetScene(ScenePath).Instantiate<NDevaFormVfx>();
        formVfx.Initialize(target.Player);
        creatureNode.Visuals.AddFormVfx(formVfx);
        return formVfx;
    }
}*/