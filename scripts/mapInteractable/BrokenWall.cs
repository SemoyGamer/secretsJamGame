using Godot;
using System;

public partial class BrokenWall : StaticBody2D
{
	private AnimatedSprite2D sprite;
	private bool destroy = false;

	public override void _Ready()
	{
		sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
	}

	public void _on_area_2d_area_entered(Area2D area){
		if(!destroy){
			sprite.Play("fade");
			destroy = true;
		}
	}

	public void _on_animated_sprite_2d_animation_finished(){
		if(destroy){
			QueueFree();
		}
		GD.Print(destroy);
	}
}
