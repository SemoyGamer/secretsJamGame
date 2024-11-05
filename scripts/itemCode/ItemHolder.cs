using Godot;
using System;

public partial class ItemHolder : Node2D
{
	[Export]
	public Area2D heldItem {get; set;}

	public bool carryingItem;

	public override void _Process(double delta)
	{
		//check if there is an item carried in its spot
		if(heldItem != null){
			carryingItem = true;
		}else{
			carryingItem = false;
		}
	}
}
