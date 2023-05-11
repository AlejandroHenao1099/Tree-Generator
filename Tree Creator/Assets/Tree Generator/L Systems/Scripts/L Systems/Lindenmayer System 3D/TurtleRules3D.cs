using UnityEngine;

[System.Serializable]
public struct TurtleRules3D
{
    public float LenghtMovement;
    public float AngleTurn;
    public float initialWidth;
    public float tropism;
    public Vector3 directionTropism;
    public bool invertRotation;
    public AlignTurtle initialAlign;

    public Matrix3x3 initialOrientationTurtle
    {
        get
        {
            switch (initialAlign)
            {
                case AlignTurtle.Forward:
                    return Matrix3x3.AlignZ;
                case AlignTurtle.Upward:
                    return Matrix3x3.AlignY;
                case AlignTurtle.Rightward:
                    return Matrix3x3.AlignX;
                default:
                    return Matrix3x3.AlignZ;
            }
        }
    }
}

public enum AlignTurtle
{
    Forward, Rightward, Upward
}
