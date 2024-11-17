using Godot;
using System;

public partial class snowFollower : GpuParticles2D
{
	public override void _Process(double delta)
	{
		Position = GetParent().GetNode<CharacterBody2D>("player").Position;
	}
}
