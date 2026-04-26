using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Watcher.Code.Nodes;

[GlobalClass]
public partial class WatcherNCreatureVisuals : NCreatureVisuals
{
    private AnimationPlayer? _eyeAnimPlayer;
    private MegaBone? _eyeBone;
    private Node2D? _eyeNode;
    private bool _eyeSetupDone;
    private AnimationNodeStateMachinePlayback? _playback;
    private CreatureAnimator? _animator;
    private CanvasItemMaterial? _premultMat;
    private Material? _oldMaterial;

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


    private void OnAnimationTrigger(string trigger)
    {
        switch (trigger)
        {
            case "Idle":
            case "Hit":
                if (_animator == null) return;
                _animator.SetTrigger(trigger);
                break;
            case "Attack":
                if (_playback == null) return;
                _playback.Travel(trigger);
                break;
            case "Dead":
                if (_playback == null) return;
                if (_oldMaterial != null)
                {
                    SpineBody?.SetNormalMaterial(_oldMaterial);
                }
                _body.Material = null;
                _playback.Travel(trigger);
                break;
        }
    }
    
    [HarmonyPatch(typeof(NCreature), nameof(NCreature.SetAnimationTrigger))]
    static class HexaghostAnimationPatch
    {
        [HarmonyPrefix]
        static bool MyAnimations(NCreature __instance, string trigger)
        {
            if (__instance.Visuals is not WatcherNCreatureVisuals hexVisuals || __instance._spineAnimator == null) return true;
            hexVisuals._animator = __instance._spineAnimator;
            hexVisuals.OnAnimationTrigger(trigger);
            return false;
        }
    }

    [HarmonyPatch(typeof(NCreature), nameof(NCreature.StartDeathAnim))]
    static class HexaghostDeathAnimPatch
    {
        [HarmonyPrefix]
        static bool MyDeathAnimation(NCreature __instance)
        {
            if (__instance.Visuals is not WatcherNCreatureVisuals hexVisuals || __instance._spineAnimator == null) return true;
            hexVisuals._animator = __instance._spineAnimator;
            hexVisuals.OnAnimationTrigger("Dead");
            return false;
        }
    }
}

