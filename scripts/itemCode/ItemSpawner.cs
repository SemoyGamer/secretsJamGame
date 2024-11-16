using Godot;
using System;

public partial class ItemSpawner : Node2D
{
	[Export]
	public PackedScene itemToSpawn{get; set;}

	[Export]
	public float timerLength{get; set;}

	Timer spawnTimer;
	
	public override void _Ready(){
		spawnTimer = GetNode<Timer>("Timer");
		spawnTimer.WaitTime = timerLength;
	}

	//spawn the packed scene at a position close to the item spawner if it can spawn
	public void _on_timer_timeout(){
		var spawnItem = (floorItems)itemToSpawn.Instantiate();
		spawnItem.Position = Position + new Vector2((float)GD.RandRange(-40.0f, 40.0f), (float)GD.RandRange(-40.0f, 40.0f));
		AddChild(spawnItem);
	}

	//when the item gets picked up, it can now spawn more items
	public void _on_child_exiting_tree(Node node){
		spawnTimer.Start();
	}
}
