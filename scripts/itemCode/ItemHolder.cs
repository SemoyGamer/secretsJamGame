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
			}else if(heldItem.itemName == "blower"){
				var blowerObj = GD.Load<PackedScene>("res://objects/itemObj/blower.tscn");
				var blowerIns = blowerObj.Instantiate();
				AddChild(blowerIns);
			}else if(heldItem.itemName == "engine"){
				var engineObj = GD.Load<PackedScene>("res://objects/itemObj/engine.tscn");
				var engineIns = engineObj.Instantiate();
				AddChild(engineIns);
			}else if(heldItem.itemName == "gunpowder"){
				var gunpowderObj = GD.Load<PackedScene>("res://objects/itemObj/gunpowder.tscn");
				var gunpowderIns = gunpowderObj.Instantiate();
				AddChild(gunpowderIns);
			}else if(heldItem.itemName == "energyStick"){
				var eStickObj = GD.Load<PackedScene>("res://objects/itemObj/energy_stick.tscn");
				var eStickIns = eStickObj.Instantiate();
				AddChild(eStickIns);
			}else if(heldItem.itemName == "heaterCore"){
				var hCoreObj = GD.Load<PackedScene>("res://objects/itemObj/heater_core.tscn");
				var hCoreIns = hCoreObj.Instantiate();
				AddChild(hCoreIns);
			}else if(heldItem.itemName == "snipper"){
				var snipperObj = GD.Load<PackedScene>("res://objects/itemObj/snipper.tscn");
				var snipperIns = snipperObj.Instantiate();
				AddChild(snipperIns);
			}else if(heldItem.itemName == "ultraFan"){
				var uFanObj = GD.Load<PackedScene>("res://objects/itemObj/ultra_fan.tscn");
				var uFanIns = uFanObj.Instantiate();
				AddChild(uFanIns);
			}else if(heldItem.itemName == "bomb"){
				var bombObj = GD.Load<PackedScene>("res://objects/itemObj/bomb.tscn");
				var bombIns = bombObj.Instantiate();
				AddChild(bombIns);
			}else if(heldItem.itemName == "heatStick"){
				var hStickObj = GD.Load<PackedScene>("res://objects/itemObj/heat_stick.tscn");
				var hStickIns = hStickObj.Instantiate();
				AddChild(hStickIns);
			}
		}
	}
}
