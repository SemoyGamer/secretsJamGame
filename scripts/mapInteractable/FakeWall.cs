using Godot;
using System;

public partial class FakeWall : StaticBody2D
{
	Timer destroyTimer;

	public override void _Ready(){
		destroyTimer = GetNode<Timer>("destroyTimer");
	}

	public void _on_area_2d_area_entered(Area2D area){
		destroyTimer.Start();
	}

	public void _on_area_2d_area_exited(Area2D area){
		destroyTimer.Stop();
	}

	public void _on_destroy_timer_timeout(){
		QueueFree();
	}
}
