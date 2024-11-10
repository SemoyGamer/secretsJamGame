using Godot;
using System;

public partial class Blower : Sprite2D
{
    public override void _Process(double delta){
        if(Input.IsActionJustPressed("use")){
            var windObj = (Area2D)GD.Load<PackedScene>("res://objects/itemObj/itemProjectiles/blower_stream.tscn").Instantiate();
            //add the scene not on the player or item holder
            GetParent().GetParent().GetParent().AddChild(windObj);
        }
    }
}