using Godot;
using System;

public partial class LargeHeater : AnimatedSprite2D
{
	AnimatedSprite2D fakePlayer;
	Timer walkTimer;

	bool isWalking = false;
	bool isEndAnim = false;

	public override void _Ready()
	{
		fakePlayer = GetNode<AnimatedSprite2D>("fakePlayer");
		walkTimer = GetNode<Timer>("walkTimer");

		fakePlayer.Visible = false;
	}

	public override void _PhysicsProcess(double delta)
	{
		if(isWalking){
			fakePlayer.Position += new Vector2(0, -3);
			fakePlayer.GetNode<Camera2D>("Camera2D").Position += new Vector2(0, -0.5f);
		}
	}

	public void _on_area_2d_body_entered(Node2D body){
		if(body.IsInGroup("player")){
			body.QueueFree();
			body.GetNode<Camera2D>("Camera2D").Enabled = false;
			fakePlayer.GetNode<Camera2D>("Camera2D").Enabled = true;
			fakePlayer.Visible = true;
			isWalking = true;

			walkTimer.Start();
		}
	}

	public void _on_walk_timer_timeout(){
		isWalking = false;
		fakePlayer.Play("idle");
		Play("waterGone");
		GetNode<AnimatedSprite2D>("offButton").Play("pressed");
		GetNode<AudioStreamPlayer2D>("waterFlowSound").Stop();
		isEndAnim = true;
	}

	public void _on_animation_finished(){
		if(isEndAnim){
			GetNode<Timer>("endTimer").Start();
		}
	}

	public void _on_end_timer_timeout(){
		GetTree().ChangeSceneToFile("res://scenes/menus/end_scene.tscn");
	}
}
