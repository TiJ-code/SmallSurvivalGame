using Godot;
using System;

public partial class PlayerMovement : CharacterBody2D
{
	private const float Speed = 50f;
	
	[Export] private AnimationTree animationTree;
	private AnimationNodeStateMachinePlayback stateMachine;

	public override void _Ready()
	{
		stateMachine = (AnimationNodeStateMachinePlayback)animationTree.Get("parameters/playback");
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
		
		Vector2 direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		direction = direction.Normalized();
		if (direction != Vector2.Zero)
		{
			velocity = direction * Speed;
			
			stateMachine.Travel("walk");
			animationTree.Set("parameters/idle/BlendSpace2D/blend_position", direction);
			animationTree.Set("parameters/walk/BlendSpace2D/blend_position", direction);
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Y = Mathf.MoveToward(Velocity.Y, 0, Speed);
			
			stateMachine.Travel("idle");
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
