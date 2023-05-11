using UnityEngine;

public class Splines
{
    private Vector3[] points;
    private bool loop;

    public Splines(Vector3[] points, bool loop)
    {
        this.points = points;
        this.loop = loop;
    }

    public int PointsCount
    {
        get
        {
            return points.Length;
        }
    }

    public Vector3 GetControlPoint(int index)
    {
        return points[index];
    }

    public int CurveCount
    {
        get
        {
            return (points.Length - 1) / 3;
        }
    }

    public Vector3 GetPoint(float t)
    {
        if (PointsCount == 2)
            return points[Mathf.RoundToInt(t)];
        else if (PointsCount == 3)
            return Bezier.GetPoint(points[0], points[1], points[2], Mathf.Clamp01(t));

        int i;
        if (t >= 1f)
        {
            t = 1f;
            i = points.Length - 4;
        }
        else
        {
            t = Mathf.Clamp01(t) * CurveCount;
            i = (int)t;
            t -= i;
            i *= 3;
        }
        return Bezier.GetPoint(points[i], points[i + 1], points[i + 2], points[i + 3], t);
    }

    public Vector3 GetVelocity(float t)
    {
        if (PointsCount == 2)
            return points[1] - points[0];
        else if (PointsCount == 3)
            return Bezier.GetFirstDerivative(points[0], points[1], points[2], Mathf.Clamp01(t));

        int i;
        if (t >= 1f)
        {
            t = 1f;
            i = points.Length - 4;
        }
        else
        {
            t = Mathf.Clamp01(t) * CurveCount;
            i = (int)t;
            t -= i;
            i *= 3;
        }
        return Bezier.GetFirstDerivative(points[i], points[i + 1], points[i + 2], points[i + 3], t);
    }

    public Vector3 GetDirection(float t)
    {
        return GetVelocity(t).normalized;
    }
}
