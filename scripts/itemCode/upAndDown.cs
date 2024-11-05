using Godot;
using System;

public partial class upAndDown : Area2D
{
	public int shakeAmount = 15;

	Vector2 StartingPos;

	[Signal]
	public delegate void movementDoneEventHandler(string type);

	public override void _Ready(){
		StartingPos = Position;
		objectUp();
	}

	public override void _Process(double delta){
		if(Position == StartingPos){
			EmitSignal(SignalName.movementDone, "objectDown");
		}
		if(Position == StartingPos - new Vector2(0, (float)shakeAmount)){
			EmitSignal(SignalName.movementDone, "objectUp");
		}
	}
	public void objectUp(){
		var Tween = CreateTween();
		Tween.TweenProperty(this, "position", Position - new Vector2(0, (float)shakeAmount), 0.5).SetTrans(Tween.TransitionType.Sine);
	}

	public void objectDown(){
		var Tween = CreateTween();
		Tween.TweenProperty(this, "position", Position - new Vector2(0, -(float)shakeAmount), 0.5).SetTrans(Tween.TransitionType.Sine);
	}

	public void _on_movement_done(string type){
		if(type == "objectUp"){
			objectDown();
		}else{
			objectUp();
		}
	}
}
