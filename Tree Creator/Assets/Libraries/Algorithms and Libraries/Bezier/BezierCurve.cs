using UnityEngine;

public static class BezierCurve
{
    // DeCasteljausAlgorithm
    public static Vector3 Create(float t, Vector3 start, Vector3 controlStart,
             Vector3 controlEnd, Vector3 end)
    {
        //Linear interpolation = lerp = (1 - t) * A + t * B
        //Could use Vector3.Lerp(A, B, t)
        //To make it faster
        float oneMinusT = 1f - t;
        //Layer 1
        Vector3 Q = oneMinusT * start + t * controlStart;
        Vector3 R = oneMinusT * controlStart + t * controlEnd;
        Vector3 S = oneMinusT * controlEnd + t * end;
        //Layer 2
        Vector3 P = oneMinusT * Q + t * R;
        Vector3 T = oneMinusT * R + t * S;
        //Final interpolated position
        Vector3 U = oneMinusT * P + t * T;
        return U;
    }
}