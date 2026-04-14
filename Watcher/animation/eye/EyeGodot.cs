using Godot;

public partial class EyeGodot : Node2D
{
	private AnimationPlayer _animPlayer;

	public override void _Ready()
	{
		_animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

		// Play default state
		SetStance("calm");
	}

	public void SetStance(string animationName)
	{
		if (_animPlayer == null) return;

		if (_animPlayer.HasAnimation(animationName))
		{
			_animPlayer.Play(animationName);
		}
		else
		{
			GD.PrintErr($"[EyeGodot] Animation '{animationName}' not found!");
		}
	}

	// List all available animations (useful for debugging)
	public void PrintAllAnimations()
	{
		foreach (var name in _animPlayer.GetAnimationList())
			GD.Print($"[EyeGodot] Animation: {name}");
	}
}
