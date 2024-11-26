using Godot;
using System;

public partial class EndScene : Node2D
{
	public void _on_back_button_pressed(){
		GetTree().ChangeSceneToFile("res://scenes/menus/main_menu.tscn");
	}
}
