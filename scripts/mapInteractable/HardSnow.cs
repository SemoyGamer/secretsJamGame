using Godot;
using System;

public partial class HardSnow : StaticBody2D
{
	CollisionShape2D collider;

	public override void _Ready()
	{
		collider = GetNode<CollisionShape2D>("playerCollider");
	}

	//uses the set deffered, which waits until it is safe to perform a physics thing, to disable or enable the snow's collision
	public void _on_heat_stick_detector_area_entered(Area2D area){
		collider.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
	}

	public void _on_heat_stick_detector_area_exited(Area2D area){
		collider.SetDeferred(CollisionShape2D.PropertyName.Disabled, false);
	}
}
