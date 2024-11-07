using Godot;
using System;

public partial class ItemHolder : Node2D
{
	[Export]
	public floorItems heldItem {get; set;}

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

	public void deleteItem(){
		GetChildren()[0].QueueFree();
		heldItem = null;
	}

	public void _on_player_item_changed(){
		if(!carryingItem){
			if(heldItem.itemName == "torch"){
				var torchObj = GD.Load<PackedScene>("res://objects/itemObj/torch.tscn");
				var torchIns = torchObj.Instantiate();
				AddChild(torchIns);
			}
		}
	}
}
