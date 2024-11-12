using Godot;
using System;

public partial class PlantWall : StaticBody2D
{
	public void _on_killer_area_area_entered(Area2D area){
		QueueFree();
	}
}
