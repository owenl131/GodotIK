using Godot;

public class Bee : Spatial
{
    public Vector3 target;
    public Timer timer;
    public MeshInstance mesh;
    public Vector3 position;

    public Bee()
    {
        target = GenerateRandomPosition();
    }


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        position = Vector3.Zero;
        SphereMesh sphere = new SphereMesh();
        sphere.Radius = 0.3f;
        sphere.Height = 0.6f;
        mesh = new MeshInstance();
        mesh.Mesh = sphere;
        mesh.MaterialOverride = new SpatialMaterial()
        {
            AlbedoColor = Colors.Red
        };
        AddChild(mesh);

        timer = new Timer();
        AddChild(timer);
        timer.Connect("timeout", this, "OnTimeout");
        timer.Start(4);
    }

    public void OnTimeout()
    {
        target = GenerateRandomPosition();
    }

    public override void _PhysicsProcess(float delta)
    {
        float speed = 3f;
        Vector3 dir = target - position;
        if (dir.LengthSquared() > 1)
            dir = dir.Normalized();
        position += dir * speed * delta;
        Transform newTransform = new Transform(Basis.Identity, position);
        GlobalTransform = newTransform;
    }
    
    public Vector3 GenerateRandomPosition()
    {
        float range = 3;
        return new Vector3(
            2 * range * GD.Randf() - range,
            2 * range * GD.Randf() - range,
            2 * range * GD.Randf() - range);
    }

}
