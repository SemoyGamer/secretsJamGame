using Godot;
using System;

public partial class Player : CharacterBody2D{
	public const float Speed = 400.0f;

	Vector2 direction;

	public override void _PhysicsProcess(double delta){
		Vector2 velocity = Velocity;

		//get input direction and move the player
		direction = Input.GetVector("left", "right", "up", "down");
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
