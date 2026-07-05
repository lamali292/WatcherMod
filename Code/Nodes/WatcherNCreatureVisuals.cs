using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Watcher.Code.Nodes;

[GlobalClass]
public partial class WatcherNCreatureVisuals : NCreatureVisuals
{
    public CreatureAnimator? Animator;
    private AnimationPlayer? _eyeAnimPlayer;
    private MegaBone? _eyeBone;
    private Node2D? _eyeNode;
    private bool _eyeSetupDone;
    private Material? _oldMaterial;
    private AnimationNodeStateMachinePlayback? _playback;
    private CanvasItemMaterial? _premultMat;

    public override void _Ready()
    {
        base._Ready();
        _oldMaterial = SpineBody?.GetNormalMaterial();
        _premultMat = new CanvasItemMaterial
        {
            BlendMode = CanvasItemMaterial.BlendModeEnum.PremultAlpha
        };
        SpineBody?.SetNormalMaterial(_premultMat);
        _body.Material = _premultMat;

        GetTree().ProcessFrame += SetupEye;
        var animTree = GetNode<AnimationTree>("%AnimationTree");
        animTree.Active = true;
        _playback = (AnimationNodeStateMachinePlayback)animTree.Get("parameters/playback");
    }

    private void SetupEye()
    {
        if (_eyeSetupDone) return;
        _eyeSetupDone = true;
        GetTree().ProcessFrame -= SetupEye;

        _eyeBone = SpineBody?.GetSkeleton()?.FindBone("eye_anchor");
        SpineBody?.ConnectWorldTransformsChanged(Callable.From<Variant>(OnEyeWorldTransformsChanged));
        _eyeNode = GetNode<Node2D>("%Eye");
        if (_eyeNode == null) return;
        _eyeAnimPlayer = _eyeNode.GetNodeOrNull<AnimationPlayer>("%EyeAnimationPlayer");
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


    public void OnAnimationTrigger(string trigger)
    {
        switch (trigger)
        {
            case "Idle":
            case "Hit":
                if (Animator == null) return;
                Animator.SetTrigger(trigger);
                break;
            case "Attack":
                if (_playback == null) return;
                _playback.Travel(trigger);
                break;
            case "Dead":
                if (_playback == null) return;
                SpineBody?.SetNormalMaterial(_oldMaterial);
                _body.Material = null;
                _playback.Travel(trigger);
                break;
        }
    }

    
}

