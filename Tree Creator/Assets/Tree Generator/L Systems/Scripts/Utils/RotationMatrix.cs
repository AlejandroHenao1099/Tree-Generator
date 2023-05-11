using UnityEngine;
using static UnityEngine.Mathf;

public static class RotationMatrix
{
    public static Matrix3x3 TurnMatrix(Matrix3x3 matrix, float angles)
    {
        var rotationMatrix = new Matrix3x3(Vector3.zero, Vector3.zero, Vector3.zero);
        rotationMatrix.SetRow(0, new Vector3(Cos(angles * Deg2Rad), Sin(angles * Deg2Rad), 0));
        rotationMatrix.SetRow(1, new Vector3(-Sin(angles * Deg2Rad), Cos(angles * Deg2Rad), 0));
        rotationMatrix.SetRow(2, new Vector3(0, 0, 1));
        return matrix * rotationMatrix;
    }

    public static Matrix3x3 PitchMatrix(Matrix3x3 matrix, float angles)
    {
        var rotationMatrix = new Matrix3x3(Vector3.zero, Vector3.zero, Vector3.zero);
        rotationMatrix.SetRow(0, new Vector3(Cos(angles * Deg2Rad), 0, -Sin(angles * Deg2Rad)));
        rotationMatrix.SetRow(1, new Vector3(0, 1, 0));
        rotationMatrix.SetRow(2, new Vector3(Sin(angles * Deg2Rad), 0, Cos(angles * Deg2Rad)));
        return matrix * rotationMatrix;
    }

    public static Matrix3x3 RollMatrix(Matrix3x3 matrix, float angles)
    {
        var rotationMatrix = new Matrix3x3(Vector3.zero, Vector3.zero, Vector3.zero);
        rotationMatrix.SetRow(0, new Vector3(1, 0, 0));
        rotationMatrix.SetRow(1, new Vector3(0, Cos(angles * Deg2Rad), -Sin(angles * Deg2Rad)));
        rotationMatrix.SetRow(2, new Vector3(0, Sin(angles * Deg2Rad), Cos(angles * Deg2Rad)));
        return matrix * rotationMatrix;
    }
}
