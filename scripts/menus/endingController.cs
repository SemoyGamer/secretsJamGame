using Godot;
using System;

public partial class endingController : Node2D
{
	Timer endTimer;
	Node2D snowPiles;
	Node2D heaters;

	Node2D rain;
	Node2D snow;

	bool isChanging = false;

	public override void _Ready()
	{
		endTimer = GetNode<Timer>("endTimer");
		snowPiles = GetNode<Node2D>("snowPiles");
		heaters = GetNode<Node2D>("heaters");
		rain = GetNode<Node2D>("rain");
		snow = GetNode<Node2D>("snow");
	}

	public override void _Process(double delta)
	{
		if(isChanging){
			snowPiles.Modulate -= new Color(0, 0, 0, 0.2f * (float)delta);
			heaters.Visible = true;
			heaters.Modulate += new Color(0, 0, 0, 0.5f * (float)delta);
			snow.Modulate -= new Color(0, 0, 0, 0.5f * (float)delta);
			rain.Modulate += new Color(0, 0, 0, 0.5f * (float)delta);
		}
	}

	public void _on_start_timer_timeout(){
		endTimer.Start();
		isChanging = true;
	}

	public void _on_end_timer_timeout(){
		GetTree().ChangeSceneToFile("res://scenes/menus/end_scene.tscn");
	}
}
