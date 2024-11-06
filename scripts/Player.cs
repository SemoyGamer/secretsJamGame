using Godot;
using System;

public partial class Player : CharacterBody2D{
	public const float Speed = 400.0f;

	Vector2 direction;

	AnimatedSprite2D playerSprite;

	//smooth rotation values
	public float _theta;
	public float rotationSpeed = Mathf.Tau * 2;

	//item holder variables
	private ItemHolder itemHolder1;
	private ItemHolder itemHolder2;
	private floorItems pickableItem = null;

	//item holder signal
	[Signal]
	public delegate void itemChangedEventHandler();

	public override void _Ready(){
		//Add a value to some of the variables
		playerSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		itemHolder1 = GetNode<ItemHolder>("itemHolder");
		itemHolder2 = GetNode<ItemHolder>("itemHolder2");
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
	}

	public override void _Process(double delta){
		//let the player pick up an item
		if(Input.IsActionJustPressed("pickUp") && pickableItem != null){
			pickUpItem(pickableItem);
		}
	}

	public void pickUpItem(Area2D item){
		//checks if the player's hands are empty so it doesn't grab something it can't
		if(itemHolder1.heldItem == null || itemHolder2.heldItem == null){
			if(itemHolder1.heldItem == null && itemHolder2.heldItem == null){
				itemHolder1.heldItem = (floorItems)item;
			}else if(itemHolder1.heldItem != null && itemHolder2.heldItem == null){
				itemHolder2.heldItem = (floorItems)item;
			}else if(itemHolder1.heldItem == null && itemHolder2.heldItem != null){
				itemHolder1.heldItem = (floorItems)item;
			}
			item.QueueFree();
			EmitSignal(SignalName.itemChanged);
		}
	}

	public void _on_pick_up_range_area_entered(Area2D item){
		pickableItem = (floorItems)item;
	}

	public void _on_pick_up_range_area_exited(Area2D item){
		pickableItem = null;
	}
}
