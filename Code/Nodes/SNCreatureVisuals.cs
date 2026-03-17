using Godot;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Watcher.Code.Nodes;

[GlobalClass]
public partial class SNCreatureVisuals : NCreatureVisuals
{
    private AnimationPlayer? _eyeAnimPlayer;
    private MegaBone? _eyeBone;
    private Node2D? _eyeNode;
    private bool _eyeSetupDone;

    public void InitEye(MegaSprite controller)
    {
        _eyeBone = controller.GetSkeleton().FindBone("eye_anchor");
        controller.ConnectWorldTransformsChanged(Callable.From<Variant>(OnEyeWorldTransformsChanged));
        GetTree().ProcessFrame += SetupEye;
    }

    private void SetupEye()
    {
        if (_eyeSetupDone) return;
        _eyeSetupDone = true;
        GetTree().ProcessFrame -= SetupEye;

        _eyeNode = ((Node)SpineBody!.BoundObject).GetNodeOrNull<Node2D>("Eye");
        if (_eyeNode == null)
        {
            GD.PrintErr("[SNCreatureVisuals] Eye node not found!");
            return;
        }

        _eyeAnimPlayer = _eyeNode.GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
        _eyeAnimPlayer?.Play("RESET");
    }

    private void OnEyeWorldTransformsChanged(Variant _)
    {
        if (_eyeNode == null || _eyeBone == null) return;
        var worldX = _eyeBone.BoundObject.Call("get_world_x").As<float>();
        var worldY = _eyeBone.BoundObject.Call("get_world_y").As<float>();
        _eyeNode.Position = new Vector2(worldX, worldY);
    }

    public void SetEyeStance(string stance)
    {
        _eyeAnimPlayer?.Play(stance); // "calm", "divinity", "wrath"
    }
}