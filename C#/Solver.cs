using Godot;
using System.Collections.Generic;
using System;

public class Solver
{
    public Joint endpoint;
    public List<Joint> joints;

    // d(endpoint) = Jacobian * d(configuration)
    public float[,] ComputeJacobian()
    {
        // Each output element corresponds to 
        // a column in the jacobian matrix
        float[,] result = new float[3, joints.Count];
        // Each column is [dx/dc, dy/dc, dz/dc] for configuration
        // variable c
        float epsilon = 1E-3f;
        Vector3 current = endpoint.GlobalTransform.origin;
        int index = 0;
        foreach (Joint j in joints)
        {
            j.SaveState();
            j.UpdateParameter(epsilon);
            Vector3 newPosition = endpoint.GlobalTransform.origin;
            j.ResetState();
            Vector3 diff = (newPosition - current) / epsilon;
            result[0, index] = diff[0];
            result[1, index] = diff[1];
            result[2, index] = diff[2];
            index++;
        }
        return result;
    }

    public float[] RandomUnitVector(int length)
    {
        float[] result = new float[length];
        for (int i = 0; i < length; i++)
        {
            result[i] = 2 * GD.Randf() - 1f;
        }
        return Vecmath.Normalized(result);
    }

    public float[] SVD1D(float[,] matrix, float eps=1E-8f)
    {
        float[] lastIteration;
        float[] currentIteration = RandomUnitVector(matrix.GetLength(1));
        float[,] b = Vecmath.Mul(Vecmath.Transpose(matrix), matrix);
        int iterations = 0;
        while (iterations < 100)
        {
            lastIteration = currentIteration;
            currentIteration = Vecmath.Mul(b, lastIteration);
            currentIteration = Vecmath.Normalized(currentIteration);
            if (Mathf.Abs(Vecmath.Dot(lastIteration, currentIteration)) > 1 - eps)
            {
                break;
            }
            iterations += 1;
        }
        return currentIteration;
    }

    public (float[], float[,], float[,]) SVD(float[,] matrix, float eps=1E-8f)
    {
        // if matrix is M x N, then S is 1 x N, U is M x M and V is N x N
        // matrix = U * S * V_transpose
        float[] sigmas = new float[matrix.GetLength(1)];
        float[,] us = new float[matrix.GetLength(0), matrix.GetLength(0)];
        float[,] vs = new float[matrix.GetLength(1), matrix.GetLength(1)];
        float[,] remaining = Vecmath.Copy(matrix);
        for (int i = 0; i < Math.Min(matrix.GetLength(0), matrix.GetLength(1)); i++)
        {
            float[] v = SVD1D(remaining, eps);
            float[] u = Vecmath.Mul(matrix, v);
            float[,] contribution = Vecmath.OuterProduct(u, v);
            float sigma = Vecmath.Length(u);
            u = Vecmath.Normalized(u);
            for (int j = 0; j < matrix.GetLength(0); j++)
            {
                us[j, i] = u[j];
            }
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                vs[j, i] = v[j];
            }
            sigmas[i] = sigma;
            remaining = Vecmath.ScaleAdd(remaining, contribution, -1);
        }
        return (sigmas, us, vs);
    }

    public float[,] ComputePseudoinverse(float[,] matrix, float eps=1E-8f)
    {
        float[,] result;
        var svd = SVD(matrix);
        float[] singulars = svd.Item1;
        float[,] u = svd.Item2, v = svd.Item3;
        // Debug.Assert(u.GetLength(0) == u.GetLength(1) && u.GetLength(0) == matrix.GetLength(0));
        // Debug.Assert(v.GetLength(0) == v.GetLength(1) && v.GetLength(0) == matrix.GetLength(1));
        for (int i = 0; i < singulars.Length; i++)
        {
            if (singulars[i] > eps)
            {
                singulars[i] = 1 / singulars[i];
            }
        }
        float[,] s = new float[v.GetLength(0), u.GetLength(0)];
        for (int i = 0; i < Math.Min(singulars.Length, Math.Min(v.GetLength(0), u.GetLength(0))); i++)
        {
            s[i, i] = singulars[i];
        }
        var partial = Vecmath.Mul(v, s);
        result = Vecmath.Mul(
            partial,
            Vecmath.Transpose(u));
        // Debug.Assert(result.GetLength(0) == matrix.GetLength(1));
        // Debug.Assert(result.GetLength(1) == matrix.GetLength(0));
        return result;
    }

    public float[] ComputeIK(Vector3 target, float damp)
    {
        float[,] jacobian = ComputeJacobian();
        Vector3 current = endpoint.GlobalTransform.origin;
        Vector3 dx = target - current;
        // OS.SetWindowTitle("Dist: " + dx.Length());
        float[] dposition = new float[]{dx.x, dx.y, dx.z};
        float[,] damped = Vecmath.ScaleAdd(
            Vecmath.Mul(Vecmath.Transpose(jacobian), jacobian),
            Vecmath.Identity(jacobian.GetLength(1)), damp * damp
        );
        float[,] invDamped = ComputePseudoinverse(damped);
        float[] jTdPosition = Vecmath.Mul(Vecmath.Transpose(jacobian), dposition);
        float[] dconfig = Vecmath.Mul(
            ComputePseudoinverse(damped), 
            jTdPosition
        );
        // jacobian is 3 x Joints, 
        return dconfig;
    }

    public float[] ComputeIK(Vector3 target)
    {
        float[,] jacobian = ComputeJacobian();
        Vector3 current = endpoint.GlobalTransform.origin;
        Vector3 dx = target - current;
        float[] dposition = new float[]{dx.x, dx.y, dx.z};
        return Vecmath.Mul(ComputePseudoinverse(jacobian), dposition);
    }

    public void TestPseudoinverse(float[,] mat)
    {
        float[,] inv = ComputePseudoinverse(mat);
        float[,] ax = Vecmath.Mul(mat, inv);
        float[,] xa = Vecmath.Mul(inv, mat);
        float[,] axa = Vecmath.Mul(ax, mat);
        float[,] xax = Vecmath.Mul(xa, inv);
        // Debug.Assert(Vecmath.IsZero(Vecmath.ScaleAdd(axa, mat, -1)));
        // Debug.Assert(Vecmath.IsZero(Vecmath.ScaleAdd(xax, inv, -1)));
        // Debug.Assert(Vecmath.IsZero(Vecmath.ScaleAdd(ax, Vecmath.Transpose(ax), -1)));
        // Debug.Assert(Vecmath.IsZero(Vecmath.ScaleAdd(xa, Vecmath.Transpose(xa), -1)));
    }

    public void TestSVD(float[,] mat)
    {
        var svd = SVD(mat);
        var singular = Vecmath.Diag(svd.Item1);
        var u = svd.Item2;
        var v = svd.Item3;
        var res = Vecmath.Mul(Vecmath.Mul(u, singular), Vecmath.Transpose(v));
        float[,] diff = Vecmath.ScaleAdd(mat, res, -1);
        // Debug.Assert(Vecmath.IsZero(diff));
    }
}
