using Godot;
using System;

public partial class Player : CharacterBody2D{
	public const float Speed = 400.0f;

	Vector2 direction;

	AnimatedSprite2D playerSprite;

	//smooth rotation values
	public float _theta;
	public float rotationSpeed = Mathf.Tau * 2;

	public override void _Ready(){
		//Add a value to some of the variables
		playerSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
	}

	public override void _PhysicsProcess(double delta){
		Vector2 velocity = Velocity;

		//get input direction and move the player, also play the walk and idle animations
		direction = Input.GetVector("left", "right", "up", "down");
		if (direction != Vector2.Zero){
			velocity = direction.Normalized() * Speed;
			playerSprite.Play("walking");

			//player rotation
			_theta = Mathf.Wrap(Mathf.Atan2(direction.Y, direction.X) - Rotation + Mathf.Pi / 2, -Mathf.Pi, Mathf.Pi);
			Rotation += Mathf.Clamp(rotationSpeed * (float)delta, 0, Mathf.Abs(_theta)) * Mathf.Sign(_theta);
		}else{
			velocity = Vector2.Zero;
			playerSprite.Play("idle");
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
