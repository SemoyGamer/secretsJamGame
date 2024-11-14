using Godot;
using System;

public partial class SetBomb : Node2D
{
	Timer setOffTimer;
	AnimatedSprite2D sprite;
	Area2D explodeWall;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		setOffTimer = GetNode<Timer>("setOffTimer");
		sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		explodeWall = GetNode<Area2D>("explodeWall");

		setOffTimer.Start();
		explodeWall.GetNode<CollisionShape2D>("CollisionShape2D").Disabled = true;
	}

	public void _on_set_off_timer_timeout(){
		sprite.Play("explosion");
		explodeWall.GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
		GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D").Play();;
	}

	public void _on_animated_sprite_2d_animation_finished(){
		QueueFree();
	}
}
