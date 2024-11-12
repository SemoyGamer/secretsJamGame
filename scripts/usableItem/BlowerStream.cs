using Godot;
using System;

public partial class BlowerStream : Area2D
{
	private bool hit = false;

	public override void _Process(double delta)
	{
		if(!hit){
			Position += new Vector2(0, -1).Rotated(Rotation) * 500 * (float)delta;
		}
	}

	public void _on_area_entered(Area2D area){
		hit = true;
		GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play("hit");
	}

	public void _on_body_entered(Node2D body){
		hit = true;
		GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play("hit");
	}

	public void _on_animated_sprite_2d_animation_finished(){
		QueueFree();
	}

	public void _on_delete_timer_timeout(){
		GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play("hit");
	}
}
