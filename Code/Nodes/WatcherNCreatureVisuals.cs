using Godot;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Watcher.Code.Nodes;

[GlobalClass]
public partial class WatcherNCreatureVisuals : NCreatureVisuals
{
    public override void _Ready()
    {
        base._Ready();

        var premultMat = new CanvasItemMaterial
        {
            BlendMode = CanvasItemMaterial.BlendModeEnum.PremultAlpha
        };

        if (SpineBody != null)
        {
            SpineBody.SetNormalMaterial(premultMat);
        }
        else
        {
            var body = GetNodeOrNull<Node2D>("%Visuals");
            if (body != null)
                body.Material = premultMat;

            var phobiaBody = GetNodeOrNull<Node2D>("%PhobiaModeVisuals");
            if (phobiaBody != null)
                phobiaBody.Material = premultMat;
        }
        //StancePower.EnsureEyeSetup(Body);
    }
}
