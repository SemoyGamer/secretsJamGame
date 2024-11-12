using Godot;
using System;

public partial class WeakWall : StaticBody2D
{
	AnimatedSprite2D sprite;

	public override void _Ready(){
		sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
	}

	public void _on_killer_area_area_entered(Area2D area){
		sprite.Play("fading");
	}

	public void _on_animated_sprite_2d_animation_finished(){
		QueueFree();
	}
}
