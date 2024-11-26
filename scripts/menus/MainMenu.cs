using Godot;
using System;

public partial class MainMenu : Node2D
{
	public void _on_play_button_pressed(){
		GetTree().ChangeSceneToFile("res://scenes/levels/snowy_bridges.tscn");
	}

	public void _on_credits_button_pressed(){
		GetTree().ChangeSceneToFile("res://scenes/menus/credits_menu.tscn");
	}

	public void _on_exit_button_pressed(){
		GetTree().Quit();
	}
}
