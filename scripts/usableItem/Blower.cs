using Godot;
using System;

public partial class Blower : Sprite2D
{
    [Export]
    public PackedScene windObj;

    public void shoot(){
        Area2D windIns = windObj.Instantiate<Area2D>();

        windIns.Rotation = GlobalRotation;
        windIns.GlobalPosition = GlobalPosition + new Vector2(0, -120).Rotated(windIns.Rotation);

        //add the scene not on the player or item holder
        GetTree().Root.AddChild(windIns);
    }
}