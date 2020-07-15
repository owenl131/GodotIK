using Godot;
using System.Diagnostics;

public class Vecmath
{
    public static float Dot(float[] v1, float[] v2)
    {
        Debug.Assert(v1.Length == v2.Length);
        float result = 0;
        for (int i = 0; i < v1.Length; i++)
        {
            result += v1[i] * v2[i];
        }
        return result;
    }

    public static float LengthSquared(float[] vector)
    {
        return Dot(vector, vector);
    }

    public static float Length(float[] vector)
    {
        return Mathf.Sqrt(LengthSquared(vector));
    }

    public static float[] ExtractRow(float[,] mat, int row)
    {
        float[] result = new float[mat.GetLength(1)];
        for (int i = 0; i < mat.GetLength(1); i++)
        {
            result[i] = mat[row, i];
        }
        return result;
    }

    public static float[] ExtractCol(float[,] mat, int col)
    {
        float[] result = new float[mat.GetLength(0)];
        for (int i = 0; i < mat.GetLength(0); i++)
        {
            result[i] = mat[i, col];
        }
        return result;
    }

    public static float[] Normalized(float[] vec, float eps=1E-6f)
    {
        float length = Length(vec);
        if (length < eps)
        {
            return vec;
        }
        for (int i = 0; i < vec.Length; i++)
        {
            vec[i] /= length;
        }
        return vec;
    }

    public static float[,] Copy(float[,] mat)
    {
        float[,] result = new float[mat.GetLength(0), mat.GetLength(1)];
        for (int i = 0; i < mat.GetLength(0); i++)
        {
            for (int j = 0; j < mat.GetLength(1); j++)
            {
                result[i, j] = mat[i, j];
            }
        }
        return result;
    }

    public static float[,] Transpose(float[,] mat)
    {
        float[,] result = new float[mat.GetLength(1), mat.GetLength(0)];
        for (int i = 0; i < mat.GetLength(0); i++)
        {
            for (int j = 0; j < mat.GetLength(1); j++)
            {
                result[j, i] = mat[i, j];
            }
        }
        return result;
    }

    public static float[] Mul(float[,] mat, float[] vec)
    {
        Debug.Assert(mat.GetLength(1) == vec.Length);
        float[] result = new float[mat.GetLength(0)];
        for (int i = 0; i < mat.GetLength(0); i++)
        {
            float[] row = ExtractRow(mat, i);
            result[i] = Dot(row, vec);
        }
        return result;
    }

    public static float[,] Mul(float[,] m1, float[,] m2)
    {
        Debug.Assert(m1.GetLength(1) == m2.GetLength(0));
        float[,] result = new float[m1.GetLength(0), m2.GetLength(1)];
        for (int i = 0; i < m1.GetLength(0); i++)
        {
            for (int j = 0; j < m2.GetLength(1); j++)
            {
                result[i, j] = Vecmath.Dot(Vecmath.ExtractRow(m1, i), Vecmath.ExtractCol(m2, j));
            }
        }
        return result;
    }

    public static float[,] Identity(int n)
    {
        float[,] result = new float[n, n];
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                result[i, j] = 0;
            }
            result[i, i] = 1;
        }
        return result;
    }

    public static void Print(float[,] mat)
    {
        GD.Print("Matrix: ", mat.GetLength(0), " ", mat.GetLength(1));
        foreach (float item in mat)
        {
            GD.Print(item);
        }
    }

    public static float[,] OuterProduct(float[] v1, float[] v2)
    {
        float[,] result = new float[v1.Length, v2.Length];
        for (int i = 0; i < v1.Length; i++)
        {
            for (int j = 0; j < v2.Length; j++)
            {
                result[i, j] = v1[i] * v2[j];
            }
        }
        return result;
    }

    public static float[,] Diag(float[] v)
    {
        float[,] result = new float[v.Length, v.Length];
        for (int i = 0; i < v.Length; i++)
        {
            for (int j = 0; j < v.Length; j++)
            {
                result[i, j] = 0;
            }
            result[i, i] = v[i];
        }
        return result;
    }

    public static float[,] ScaleAdd(float[,] mat1, float[,] mat2, float s)
    {
        Debug.Assert(mat1.GetLength(0) == mat2.GetLength(0));
        Debug.Assert(mat1.GetLength(1) == mat2.GetLength(1));
        for (int i = 0; i < mat1.GetLength(0); i++)
        {
            for (int j = 0; j < mat1.GetLength(1); j++)
            {
                mat1[i, j] += mat2[i, j] * s;
            }
        }
        return mat1;
    }

    public static bool IsZero(float[,] mat, float eps=1E-4f)
    {
        foreach (float item in mat)
        {
            if (Mathf.Abs(item) > eps)
            {
                return false;
            }
        }
        return true;
    }
}
