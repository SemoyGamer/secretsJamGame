using Godot;
using System;

public partial class Player : CharacterBody2D{
	public const float Speed = 400.0f;

	Vector2 direction;
	private bool canMove = true;
	private bool dashing = false;
	private bool died = false;

	AnimatedSprite2D playerSprite;
	Timer craftTimer;
	Timer fanDashTimer;
	Timer respawnTimer;
	AudioStreamPlayer2D soundComplete;
	AudioStreamPlayer2D soundError;
	AudioStreamPlayer footsteps;

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
		fanDashTimer = GetNode<Timer>("fanDashTimer");
		respawnTimer = GetNode<Timer>("respawnTimer");

		itemHolder1 = GetNode<ItemHolder>("itemHolder");
		iHPos = itemHolder1.Position;
		itemHolder2 = GetNode<ItemHolder>("itemHolder2");
		iHPos2 = itemHolder2.Position;

		soundComplete = GetNode<AudioStreamPlayer2D>("soundComplete");
		soundError = GetNode<AudioStreamPlayer2D>("soundError");
		footsteps = GetNode<AudioStreamPlayer>("footsteps");

		//starts the craft timer
		craftTimer.Start();
	}

	public override void _PhysicsProcess(double delta){
		Vector2 velocity = Velocity;

		//get input direction and move the player, also play the walk and idle animations
		if(canMove){
			direction = Input.GetVector("left", "right", "up", "down");
		}else{
			direction = Vector2.Zero;
		}
		
		if (direction != Vector2.Zero){
			velocity = direction.Normalized() * Speed;
			
			playerSprite.Play("walking");

			//play the footsteps sound effect if the player is walking
			if(!footsteps.Playing){
				footsteps.Play();
			}

			//player rotation
			_theta = Mathf.Wrap(Mathf.Atan2(direction.Y, direction.X) - Rotation + Mathf.Pi / 2, -Mathf.Pi, Mathf.Pi);
			Rotation += Mathf.Clamp(rotationSpeed * (float)delta, 0, Mathf.Abs(_theta)) * Mathf.Sign(_theta);
		}else{
			if(!dashing){
				velocity = Vector2.Zero;
			}
			playerSprite.Play("idle");

			//stop the footsteps sound effect if the player is not walking
			if(footsteps.Playing){
				footsteps.Stop();
			}
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

		//dash code
		if(fanDashTimer.TimeLeft > 0){
			canMove = false;
			dashing = true;
			Velocity = new Vector2(0, -1000).Rotated(Rotation);
		}else{
			canMove = true;
			dashing = false;
		}

		//death
		if(died){
			canMove = false;
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

		//using items code
		if(itemHolder1.carryingItem && !itemHolder2.carryingItem){
			if(Input.IsActionJustPressed("use")){
				//blower usage
				if(itemHolder1.heldItem.itemName == "blower"){
					itemHolder1.GetNode<Blower>("blower").shoot();
				}

				//ultra Fan usage
				if(itemHolder1.heldItem.itemName == "ultraFan" && fanDashTimer.IsStopped()){
					fanDashTimer.Start();
					itemHolder1.GetNode<Sprite2D>("ultraFan").GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D").Play();
				}
				
				//bomb usage
				if(itemHolder1.heldItem.itemName == "bomb"){
					var setBomb = (Node2D)GD.Load<PackedScene>("res://objects/itemObj/itemProjectiles/set_bomb.tscn").Instantiate();
					setBomb.Position = Position;
					GetTree().Root.AddChild(setBomb);

					itemHolder1.deleteItem();
				}

				//snipper usage
				if(itemHolder1.heldItem.itemName == "snipper"){
					itemHolder1.GetNode<Snipper>("snipper").snip();
				}
			}
		}else if(itemHolder1.carryingItem && itemHolder2.carryingItem){
			var handToUse = itemHolder1;
			if(Input.IsActionJustPressed("use")){
				//if both hands are usable, use the first hand
				if(itemHolder1.heldItem.usable && itemHolder2.heldItem.usable){
					handToUse = itemHolder1;
				}else if(itemHolder1.heldItem.usable && !itemHolder2.heldItem.usable){
					//if hand one is only usable, use it
					handToUse = itemHolder1;
				}else if(!itemHolder1.heldItem.usable && itemHolder2.heldItem.usable){
					//if hand two is only usable, use it
					handToUse = itemHolder2;
				}

				//blower usage
				if(handToUse.heldItem.itemName == "blower"){
					handToUse.GetNode<Blower>("blower").shoot();
				}else if(handToUse.heldItem.itemName == "ultraFan" && fanDashTimer.IsStopped()){
					//ultra fan usage
					fanDashTimer.Start();
					handToUse.GetNode<Sprite2D>("ultraFan").GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D").Play();
				}else if(handToUse.heldItem.itemName == "bomb"){
					//bomb usage
					var setBomb = (Node2D)GD.Load<PackedScene>("res://objects/itemObj/itemProjectiles/set_bomb.tscn").Instantiate();
					setBomb.Position = Position;
					GetTree().Root.AddChild(setBomb);

					handToUse.deleteItem();
				}else if(handToUse.heldItem.itemName == "snipper"){
					handToUse.GetNode<Snipper>("snipper").snip();
				}
			}
		}else if(!itemHolder1.carryingItem && itemHolder2.carryingItem){
			if(Input.IsActionJustPressed("use")){
				//blower usage
				if(itemHolder2.heldItem.itemName == "blower"){
					itemHolder2.GetNode<Blower>("blower").shoot();
				}

				//ultra Fan usage
				if(itemHolder2.heldItem.itemName == "ultraFan" && fanDashTimer.IsStopped()){
					fanDashTimer.Start();
					itemHolder2.GetNode<Sprite2D>("ultraFan").GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D").Play();
				}
					
				//bomb usage
				if(itemHolder2.heldItem.itemName == "bomb"){
					var setBomb = (Node2D)GD.Load<PackedScene>("res://objects/itemObj/itemProjectiles/set_bomb.tscn").Instantiate();
					setBomb.Position = Position;
					GetTree().Root.AddChild(setBomb);

					itemHolder2.deleteItem();
				}

				//snipper usage
				if(itemHolder2.heldItem.itemName == "snipper"){
					itemHolder2.GetNode<Snipper>("snipper").snip();
				}
			}
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
			}else if(itemHolder1.heldItem == null && itemHolder2.heldItem != null){
				//if hand 2 is full and 1 is empty, add the item to hand 1
				itemHolder1.heldItem = (floorItems)item;
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
		if((itemHolder1.heldItem.itemName == "blower" && itemHolder2.heldItem.itemName == "engine") || (itemHolder1.heldItem.itemName == "engine" && itemHolder2.heldItem.itemName == "blower")){
			//craft the ultra fan
			itemHolder1.deleteItem();
			itemHolder2.deleteItem();

			soundComplete.Play();

			var newItem = (Area2D)GD.Load<PackedScene>("res://objects/itemObj/floorItems/ultra_fan_floor.tscn").Instantiate();
			newItem.Position = Position;

			GetParent().GetNode<Node2D>("floorItemLayer").AddChild(newItem);
		}else if((itemHolder1.heldItem.itemName == "gunpowder" && itemHolder2.heldItem.itemName == "heaterCore") || (itemHolder1.heldItem.itemName == "heaterCore" && itemHolder2.heldItem.itemName == "gunpowder")){
			//craft the bomb
			itemHolder1.deleteItem();
			itemHolder2.deleteItem();

			soundComplete.Play();

			var newItem = (Area2D)GD.Load<PackedScene>("res://objects/itemObj/floorItems/bomb_floor.tscn").Instantiate();
			newItem.Position = Position;

			GetParent().GetNode<Node2D>("floorItemLayer").AddChild(newItem);
		}else if((itemHolder1.heldItem.itemName == "heaterCore" && itemHolder2.heldItem.itemName == "energyStick") || (itemHolder1.heldItem.itemName == "energyStick" && itemHolder2.heldItem.itemName == "heaterCore")){
			//craft the heat stick
			itemHolder1.deleteItem();
			itemHolder2.deleteItem();

			soundComplete.Play();

			var newItem = (Area2D)GD.Load<PackedScene>("res://objects/itemObj/floorItems/heat_stick_floor.tscn").Instantiate();
			newItem.Position = Position;

			GetParent().GetNode<Node2D>("floorItemLayer").AddChild(newItem);
		}else{
			soundError.Play();
		}
	}

	public void _on_fall_area_area_entered(Area2D area){
		var pVariables = GetNode<GlobalVariables>("/root/GlobalVariables");
		//set the player's checkpoint
		if(area.CollisionLayer == 8){
			pVariables.pSpawnPoint = area.Position;
		}

		//make the player respawn at the last checkpoint
		if(area.CollisionLayer == 16 && !dashing){
			respawnTimer.Start();
			Position = pVariables.pSpawnPoint;
			died = true;
		}
	}

	public void _on_respawn_timer_timeout(){
		died = false;
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