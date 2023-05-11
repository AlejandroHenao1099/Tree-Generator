using UnityEngine;

public static class Algebra
{
    public static float Slope(Vector2 a, Vector2 b)
    {
        if ((a.x == 0 && b.x == 0) || a.x == b.x) return float.MaxValue;
        return (b.y - a.y) / (b.x - a.x);
    }

    public static float SlopeYZ(Vector3 a, Vector3 b)
    {
        if ((a.z == 0 && b.z == 0) || a.z == b.z) return float.MaxValue;
        return (b.y - a.y) / (b.z - a.z);
    }

    public static float RectPointSlope(float x, Vector2 point, float slope)
    {
        // y - y1 = m(x - x1)
        // y = m(x - x1) + y1
        return (slope * (x - point.x)) + point.y;
    }

    public static float RectPointSlopeYZ(float z, Vector3 point, float slope)
    {
        // y - y1 = m(x - x1)
        // y = m(x - x1) + y1
        return (slope * (z - point.z)) + point.y;
    }
}
