using Godot;
using System;

public class Arm: Spatial
{
    public Joint parent;
    public float length;
    public MeshInstance mesh;

    public Arm()
    {

    }

    public Arm(Joint parent, float length=2.0f)
    {
        this.length = length;

        if (parent != null)
            parent.AddChild(this);

        CylinderMesh cylinder = new CylinderMesh();
        cylinder.Height = length;
        cylinder.TopRadius = 0.1f;
        cylinder.BottomRadius = 0.1f;
        mesh = new MeshInstance();
        mesh.Translation = Vector3.Up * (length / 2);
        mesh.Mesh = cylinder;
        mesh.MaterialOverride = new SpatialMaterial()
        {
            AlbedoColor = Colors.GreenYellow
        };

        AddChild(mesh);
    }

    public Vector3 GetEndpoint()
    {
        return Vector3.Up * length;
    }
}
