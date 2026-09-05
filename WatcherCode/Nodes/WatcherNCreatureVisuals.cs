using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Watcher.Code.Nodes;

[GlobalClass]
public partial class WatcherNCreatureVisuals : NCreatureVisuals
{
	private Node2D? _eye;
	private MegaSprite? SpineEye { get; set; }

	private bool IsSpineEye => IsInstanceValid(_eye) && _eye.GetClass() == "SpineSprite";

	private SpineAnimationAccess SpineEyeAnimation => new(SpineEye);


	public override void _Ready()
	{
		base._Ready();
		_eye = _body.GetNodeOrNull<Node2D>("%SpineEye");

		if (!IsSpineEye) return;
		SpineEye = new MegaSprite((Variant) (GodotObject) _eye);
		if (SpineEye.GetSkeleton()?.GetData() == null)
		{
			GD.PushWarning($"Spine eye skeleton data failed to load for {Name}, disabling spine animation.");
			SpineEye = null;
		}
		SpineEyeAnimation.SetAnimation("None");
	}
	

	public void SetEyeStance(string stance)
	{
		SpineEyeAnimation.SetAnimation(stance);
	}

	
}
