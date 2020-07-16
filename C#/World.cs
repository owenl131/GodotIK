using Godot;
using System.Collections.Generic;

public class World : Spatial
{
    public Solver solver;
    public Joint endpoint;
    public Bee bee;

    public override void _Ready()
    {
        Arm arm1 = new Arm(null, 0);
        AddChild(arm1);

        List<Joint> joints = new List<Joint>();
        
        Arm arm = arm1;
        joints.Add(new Joint(arm, Vector3.Right));
        arm = new Arm(joints[joints.Count - 1], 0);
        joints.Add(new Joint(arm, Vector3.Up));
        arm = new Arm(joints[joints.Count - 1]);
        joints.Add(new Joint(arm, Vector3.Forward));
        arm = new Arm(joints[joints.Count - 1]);
        joints.Add(new Joint(arm, Vector3.Right));
        arm = new Arm(joints[joints.Count - 1]);
        endpoint = new Joint(arm, Vector3.Up);
        endpoint.mesh.MaterialOverride = new SpatialMaterial() 
        {
            AlbedoColor = Colors.Blue
        };

        solver = new Solver()
        {
            endpoint = endpoint,
            joints = joints
        };

        // solver.TestSVD(new float[,]{{1, 0}, {2, 0}});
        // solver.TestSVD(new float[,]{{1, 2}, {3, 4}});
        // solver.TestSVD(new float[,]{{1, 1}, {1, 1}});
        // solver.TestSVD(new float[,]{{1, 3, 1}, {2, 4, 2}, {1, 3, 1}});
        // solver.TestPseudoinverse(new float[,]{{1, 0}, {2, 0}});
        // solver.TestPseudoinverse(new float[,]{{1, 2}, {3, 4}});
        // solver.TestPseudoinverse(new float[,]{{1, 1}, {1, 1}});

        bee = new Bee();
        AddChild(bee);
    }

    public override void _PhysicsProcess(float delta)
    {
        float scale = 2f;
        float[] drotation = solver.ComputeIK(bee.GlobalTransform.origin, 0.2f);
        for (int i = 0; i < drotation.Length; i++)
        {
            Joint joint = solver.joints[i];
            joint.UpdateParameter(delta * drotation[i] * scale);
        }
    }

}
