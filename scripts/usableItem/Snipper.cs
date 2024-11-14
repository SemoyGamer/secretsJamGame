using Godot;
using System;

public partial class Snipper : Sprite2D
{
    Area2D snipArea;
    Timer snipTimer;

    private bool snipping = false;

    public override void _Ready(){
        snipArea = GetNode<Area2D>("plantDetector");
        snipTimer = GetNode<Timer>("snipTimer");

        snipArea.GetNode<CollisionShape2D>("CollisionShape2D").Disabled = true;
    }

    public void snip(){
        if(!snipping){
            snipArea.GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
            snipTimer.Start();
            GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D").Play();
            snipping = true;;
        }
    }

    public void _on_snip_timer_timeout(){
        snipping = false;
        snipArea.GetNode<CollisionShape2D>("CollisionShape2D").Disabled = true;
    }
}
