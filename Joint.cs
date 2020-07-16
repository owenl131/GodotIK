using Godot;

public class Joint : Spatial
{
    public Arm parent;
    public Vector3 rotationAxis;
    public float rotationParameter;
    public float savedParameter;
    public MeshInstance mesh;

    public Joint(Arm parent, Vector3 rotationAxis)
    {
        this.parent = parent;
        this.rotationAxis = rotationAxis;

        SphereMesh sphere = new SphereMesh();
        sphere.Radius = 0.2f;
        sphere.Height = 0.4f;
        mesh = new MeshInstance();
        mesh.Mesh = sphere;
        AddChild(mesh);

        parent.AddChild(this);
        SetParameter(0);
    }

    public void SetParameter(float param)
    {
        rotationParameter = param;
        Transform = new Transform(
            new Quat(rotationAxis, rotationParameter),
            parent.GetEndpoint());
    }

    public void UpdateParameter(float dtheta)
    {
        SetParameter(rotationParameter + dtheta);
    }

    public void SaveState()
    {
        savedParameter = rotationParameter;
    }

    public void ResetState()
    {
        SetParameter(savedParameter);
    }


}
