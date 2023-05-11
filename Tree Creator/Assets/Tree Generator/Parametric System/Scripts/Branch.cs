using UnityEngine;

[System.Serializable]
public struct Branch
{
    private Vector3[] points;
    private Matrix4x4 orientation;

    public Branch(Vector3[] points, Vector3 up)
    {
        this.points = points;
        orientation = new Matrix4x4();
        var currBase = points[0];
        var currDirection = (points[points.Length - 1] - points[0]).normalized;
        var currRotation = Quaternion.LookRotation(up, currDirection);
        orientation.SetTRS(currBase, currRotation, Vector3.one);
    }

    public Vector3[] GetPoints()
    {
        return points;
    }

    public Vector3[] Rotate(float angle, Axis axis, int start = 0, bool updatePoints = false)
    {
        var clonedPoints = ClonePoints();
        start = start >= clonedPoints.Length - 1 ? clonedPoints.Length - 2 : start;

        for (int i = start + 1; i < clonedPoints.Length; i++)
        {
            var currPoint = clonedPoints[i];
            var currDistance = (currPoint - clonedPoints[start]).magnitude;
            var normalizePoint = (currPoint - clonedPoints[start]).normalized;
            var newPos = Vector3.zero;
            switch (axis)
            {
                case Axis.AxisY:
                    newPos = Quaternion.AngleAxis(angle, GetUp()) * normalizePoint;
                    break;
                case Axis.AxisX:
                    newPos = Quaternion.AngleAxis(angle, GetRight()) * normalizePoint;
                    break;
                case Axis.AxisZ:
                    newPos = Quaternion.AngleAxis(angle, GetForward()) * normalizePoint;
                    break;
                default:
                    newPos = Vector3.zero;
                    break;
            }
            newPos = newPos * currDistance;

            clonedPoints[i] = clonedPoints[start] + newPos;
        }

        if (updatePoints)
            Update(clonedPoints);
        return clonedPoints;
    }

    public Vector3[] Rotate(float angle, Vector3 rotationAxis, int start = 0, bool updatePoints = false)
    {
        var clonedPoints = ClonePoints();
        start = start >= clonedPoints.Length - 1 ? clonedPoints.Length - 2 : start;

        for (int i = start + 1; i < clonedPoints.Length; i++)
        {
            var currPoint = clonedPoints[i];
            var currDistance = (currPoint - clonedPoints[start]).magnitude;
            var normalizePoint = (currPoint - clonedPoints[start]).normalized;
            var newPos = Vector3.zero;
            newPos = Quaternion.AngleAxis(angle, rotationAxis) * normalizePoint;
            newPos = newPos * currDistance;

            clonedPoints[i] = clonedPoints[start] + newPos;
        }

        if (updatePoints)
            Update(clonedPoints);
        return clonedPoints;
    }

    private void Update(Vector3[] newPoints)
    {
        points = newPoints;
        var currBase = points[0];
        var currDirection = (newPoints[points.Length - 1] - points[0]).normalized;
        var currRotation = Quaternion.LookRotation(GetUp(), currDirection);
        orientation.SetTRS(currBase, currRotation, Vector3.one);
    }

    private Vector3[] ClonePoints()
    {
        var clonedPoints = new Vector3[points.Length];
        for (int i = 0; i < points.Length; i++)
            clonedPoints[i] = points[i];
        return clonedPoints;
    }

    private Vector3 GetUp()
    {
        return orientation.MultiplyVector(Vector3.forward);
    }

    private Vector3 GetForward()
    {
        return orientation.MultiplyVector(Vector3.up);
    }

    private Vector3 GetRight()
    {
        return orientation.MultiplyVector(Vector3.right);
    }
}

public enum Axis
{
    AxisX, AxisY, AxisZ
}