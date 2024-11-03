using Godot;
using System;

public partial class Player : CharacterBody2D
{
	public const float Speed = 300.0f;

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		//get input direction and move the player
		Vector2 direction = Input.GetVector("left", "right", "up", "down");
		if (direction != Vector2.Zero){
			velocity = direction.Normalized() * Speed;
		}
		else{
			velocity = Vector2.Zero;
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
