using Godot;
using System;

public partial class Player : CharacterBody2D{
	public const float Speed = 400.0f;

	Vector2 direction;

	AnimatedSprite2D playerSprite;
	Timer craftTimer;

	//smooth rotation values
	public float _theta;
	public float rotationSpeed = Mathf.Tau * 2;

	//item holder variables
	private ItemHolder itemHolder1;
	private ItemHolder itemHolder2;
	private floorItems pickableItem = null;
	private Vector2 iHPos;
	private Vector2 iHPos2;

	//item holder signal
	[Signal]
	public delegate void itemChangedEventHandler();

	public override void _Ready(){
		//Add a value to some of the variables
		playerSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		craftTimer = GetNode<Timer>("craftTimer");

		itemHolder1 = GetNode<ItemHolder>("itemHolder");
		iHPos = itemHolder1.Position;
		itemHolder2 = GetNode<ItemHolder>("itemHolder2");
		iHPos2 = itemHolder2.Position;

		//starts the craft timer
		craftTimer.Start();
	}

	public override void _PhysicsProcess(double delta){
		Vector2 velocity = Velocity;

		//get input direction and move the player, also play the walk and idle animations
		direction = Input.GetVector("left", "right", "up", "down");
		if (direction != Vector2.Zero){
			velocity = direction.Normalized() * Speed;
			playerSprite.Play("walking");

			//player rotation
			_theta = Mathf.Wrap(Mathf.Atan2(direction.Y, direction.X) - Rotation + Mathf.Pi / 2, -Mathf.Pi, Mathf.Pi);
			Rotation += Mathf.Clamp(rotationSpeed * (float)delta, 0, Mathf.Abs(_theta)) * Mathf.Sign(_theta);
		}else{
			velocity = Vector2.Zero;
			playerSprite.Play("idle");
		}

		Velocity = velocity;
		MoveAndSlide();

		//get the overlapping areas of the pickup range
		if(GetNode<Area2D>("pickUpRange").GetOverlappingAreas().Count > 0){
			pickableItem = (floorItems)GetNode<Area2D>("pickUpRange").GetOverlappingAreas()[0];
		}else{
			pickableItem = null;
		}

		//make the items shake in the players hand while crafting
		if(!craftTimer.Paused){
			itemHolder1.Position = new Vector2(iHPos.X + (float)GD.RandRange(-4.0f, 4.0f), iHPos.Y + (float)GD.RandRange(-4.0f, 4.0f));
			itemHolder2.Position = new Vector2(iHPos2.X + (float)GD.RandRange(-4.0f, 4.0f), iHPos2.Y + (float)GD.RandRange(-4.0f, 4.0f));
		}else{
			itemHolder1.Position = iHPos;
			itemHolder2.Position = iHPos2;
		}
	}

	public override void _Process(double delta){
		//let the player pick up an item
		if(Input.IsActionJustPressed("pickUp") && pickableItem != null){
			pickUpItem(pickableItem);
		}

		//let the player drop an item
		if(Input.IsActionJustPressed("drop")){
			dropItem();
		}

		//let the player use the craft button
		if(itemHolder1.carryingItem && itemHolder2.carryingItem && Input.IsActionPressed("craft")){
			craftTimer.Paused = false;
		}else{
			//resets the timer
			craftTimer.WaitTime = 2.0f;
			craftTimer.Paused = true;
		}
	}

	public void pickUpItem(Area2D item){
		//checks if the player's hands are empty so it doesn't grab something it can't
		if(itemHolder1.heldItem == null || itemHolder2.heldItem == null){
			if(itemHolder1.heldItem == null && itemHolder2.heldItem == null){
				//if both hands are empty, add the item to hand 1
				itemHolder1.heldItem = (floorItems)item;
			}else if(itemHolder1.heldItem != null && itemHolder2.heldItem == null){
				//if hand 1 is full and 2 is empty, add the item to hand 2
				itemHolder2.heldItem = (floorItems)item;
			}
			item.QueueFree();
			EmitSignal(SignalName.itemChanged);
		}
	}

	public void dropItem(){
		if(itemHolder1.heldItem != null || itemHolder2.heldItem != null){
			if(itemHolder1.heldItem != null && itemHolder2.heldItem == null){
				//if there is an item in hand 1, and not in 2, then drop it
				chooseItemToDrop(itemHolder1);
				itemHolder1.deleteItem();
			}else if(itemHolder1.heldItem != null && itemHolder2.heldItem != null){
				//If both hands are full, then drop the second hand's item
				chooseItemToDrop(itemHolder2);
				itemHolder2.deleteItem();
			}
		}
	}

	public void _on_craft_timer_timeout(){
		craft();
	}

	public void craft(){
		//check what the player is holding, then turn it into a new item
		//craft the ultra fan
		if((itemHolder1.heldItem.itemName == "blower" && itemHolder2.heldItem.itemName == "engine") || (itemHolder1.heldItem.itemName == "engine" && itemHolder2.heldItem.itemName == "blower")){
			itemHolder1.deleteItem();
			itemHolder2.deleteItem();

			var newItem = (Area2D)GD.Load<PackedScene>("res://objects/itemObj/floorItems/ultra_fan_floor.tscn").Instantiate();
			newItem.Position = Position;

			GetParent().GetNode<Node2D>("floorItemLayer").AddChild(newItem);
		}

		//craft the bomb
		if((itemHolder1.heldItem.itemName == "gunpowder" && itemHolder2.heldItem.itemName == "heaterCore") || (itemHolder1.heldItem.itemName == "heaterCore" && itemHolder2.heldItem.itemName == "gunpowder")){
			itemHolder1.deleteItem();
			itemHolder2.deleteItem();

			var newItem = (Area2D)GD.Load<PackedScene>("res://objects/itemObj/floorItems/bomb_floor.tscn").Instantiate();
			newItem.Position = Position;

			GetParent().GetNode<Node2D>("floorItemLayer").AddChild(newItem);
		}

		//craft the heat stick
		if((itemHolder1.heldItem.itemName == "heaterCore" && itemHolder2.heldItem.itemName == "energyStick") || (itemHolder1.heldItem.itemName == "energyStick" && itemHolder2.heldItem.itemName == "heaterCore")){
			itemHolder1.deleteItem();
			itemHolder2.deleteItem();

			var newItem = (Area2D)GD.Load<PackedScene>("res://objects/itemObj/floorItems/heat_stick_floor.tscn").Instantiate();
			newItem.Position = Position;

			GetParent().GetNode<Node2D>("floorItemLayer").AddChild(newItem);
		}
	}

	public void chooseItemToDrop(ItemHolder iHolder){
		if(iHolder.heldItem.itemName == "torch"){
			var itemToDrop = GD.Load<PackedScene>("res://objects/itemObj/floorItems/torch_floor.tscn");
			var itemToDropIns = (Area2D)itemToDrop.Instantiate();
			itemToDropIns.Position = Position;
			GetParent().GetNode<Node2D>("floorItemLayer").AddChild(itemToDropIns);
		}else if(iHolder.heldItem.itemName == "blower"){
			var itemToDrop = GD.Load<PackedScene>("res://objects/itemObj/floorItems/blower_floor.tscn");
			var itemToDropIns = (Area2D)itemToDrop.Instantiate();
			itemToDropIns.Position = Position;
			GetParent().GetNode<Node2D>("floorItemLayer").AddChild(itemToDropIns);
		}else if(iHolder.heldItem.itemName == "engine"){
			var itemToDrop = GD.Load<PackedScene>("res://objects/itemObj/floorItems/engine_floor.tscn");
			var itemToDropIns = (Area2D)itemToDrop.Instantiate();
			itemToDropIns.Position = Position;
			GetParent().GetNode<Node2D>("floorItemLayer").AddChild(itemToDropIns);
		}else if(iHolder.heldItem.itemName == "gunpowder"){
			var itemToDrop = GD.Load<PackedScene>("res://objects/itemObj/floorItems/gunpowder_floor.tscn");
			var itemToDropIns = (Area2D)itemToDrop.Instantiate();
			itemToDropIns.Position = Position;
			GetParent().GetNode<Node2D>("floorItemLayer").AddChild(itemToDropIns);
		}else if(iHolder.heldItem.itemName == "energyStick"){
			var itemToDrop = GD.Load<PackedScene>("res://objects/itemObj/floorItems/energy_stick_floor.tscn");
			var itemToDropIns = (Area2D)itemToDrop.Instantiate();
			itemToDropIns.Position = Position;
			GetParent().GetNode<Node2D>("floorItemLayer").AddChild(itemToDropIns);
		}else if(iHolder.heldItem.itemName == "heaterCore"){
			var itemToDrop = GD.Load<PackedScene>("res://objects/itemObj/floorItems/heater_core_floor.tscn");
			var itemToDropIns = (Area2D)itemToDrop.Instantiate();
			itemToDropIns.Position = Position;
			GetParent().GetNode<Node2D>("floorItemLayer").AddChild(itemToDropIns);
		}else if(iHolder.heldItem.itemName == "snipper"){
			var itemToDrop = GD.Load<PackedScene>("res://objects/itemObj/floorItems/snipper_floor.tscn");
			var itemToDropIns = (Area2D)itemToDrop.Instantiate();
			itemToDropIns.Position = Position;
			GetParent().GetNode<Node2D>("floorItemLayer").AddChild(itemToDropIns);
		}else if(iHolder.heldItem.itemName == "ultraFan"){
			var itemToDrop = GD.Load<PackedScene>("res://objects/itemObj/floorItems/ultra_fan_floor.tscn");
			var itemToDropIns = (Area2D)itemToDrop.Instantiate();
			itemToDropIns.Position = Position;
			GetParent().GetNode<Node2D>("floorItemLayer").AddChild(itemToDropIns);
		}else if(iHolder.heldItem.itemName == "bomb"){
			var itemToDrop = GD.Load<PackedScene>("res://objects/itemObj/floorItems/bomb_floor.tscn");
			var itemToDropIns = (Area2D)itemToDrop.Instantiate();
			itemToDropIns.Position = Position;
			GetParent().GetNode<Node2D>("floorItemLayer").AddChild(itemToDropIns);
		}else if(iHolder.heldItem.itemName == "heatStick"){
			var itemToDrop = GD.Load<PackedScene>("res://objects/itemObj/floorItems/heat_stick_floor.tscn");
			var itemToDropIns = (Area2D)itemToDrop.Instantiate();
			itemToDropIns.Position = Position;
			GetParent().GetNode<Node2D>("floorItemLayer").AddChild(itemToDropIns);
		}
	}
}
