using System.Reflection;
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

	// SetAnimation returns void on newer game builds and MegaTrackEntry on 0.107.x. The return type is part of the
	// call signature, so a direct call compiled against one branch throws MissingMethodException on the other.
	private static readonly MethodInfo SetAnimationMethod = typeof(SpineAnimationAccess).GetMethods()
		.First(m => m.Name == "SetAnimation" && m.GetParameters()[0].ParameterType == typeof(string));

	private void PlayEyeAnimation(string animation)
	{
		var ps = SetAnimationMethod.GetParameters();
		var args = new object?[ps.Length];
		args[0] = animation;
		for (var i = 1; i < ps.Length; i++)
			args[i] = ps[i].HasDefaultValue ? ps[i].DefaultValue : null;
		object boxed = SpineEyeAnimation; // struct or class: box once so the call hits the same instance
		SetAnimationMethod.Invoke(boxed, args);
	}


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
		PlayEyeAnimation("None");
	}
	

	public void SetEyeStance(string stance)
	{
		PlayEyeAnimation(stance);
	}

	
}
