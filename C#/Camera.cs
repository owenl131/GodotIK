using Godot;
using System;

public class Camera : Godot.Camera
{
    public override void _Ready()
    {
        LookAt(Vector3.Zero, Vector3.Up);
    }
}
