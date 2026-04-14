using Godot;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Nodes.Screens.Shops;

namespace Watcher.Code.Nodes;

[GlobalClass]
public partial class WatcherNMerchantCharacter : NMerchantCharacter
{
    public override void _Ready()
    {
        base._Ready();

        // Fix dark seams: atlas uses premultiplied alpha data,
        // so the spine sprite must use PremultAlpha blend mode
        var premultMat = new CanvasItemMaterial
        {
            BlendMode = CanvasItemMaterial.BlendModeEnum.PremultAlpha
        };
        var spineBody = new MegaSprite((Variant)(GodotObject)GetChild(0));
        spineBody.SetNormalMaterial(premultMat);


        PlayAnimation("Idle", true);
    }
}