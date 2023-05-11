using UnityEngine;

public static class DuneErosion
{
    private static float peekAngle = 50f;
    private static float angleSleep = 30f;
    private static float deltaGravity = 0.01f;
    private static float sandAccumulate;
    // public float deltaSlopeWind = 0.05f;
    // public float deltaHeightWind = 0.09f;

    public static void Erosion(float[,] heightMap, int iterations, float angleSleep = 30f, float deltaGravity = 0.1f
        , float peekAngle = 50f)
    {
        DuneErosion.deltaGravity = deltaGravity;
        DuneErosion.angleSleep = angleSleep;
        DuneErosion.peekAngle = peekAngle;
        int resolution = heightMap.GetLength(0);
        for (int i = 0; i < iterations; i++)
            IteratePoints(resolution, heightMap);
    }

    private static void IteratePoints(int resolution, float[,] heightMap)
    {
        float slope0 = 0;
        float slope1 = 0;
        for (int i = 0; i < resolution; i++)
        {
            sandAccumulate = 0;
            slope0 = heightMap[i, 1] - heightMap[i, 0];
            if (slope0 < 0)
                NegativeSlope(i, 0, heightMap);

            for (int j = 1; j < resolution - 1; j++)
            {
                slope0 = heightMap[i, j] - heightMap[i, j - 1];
                slope1 = heightMap[i, j + 1] - heightMap[i, j];
                if (slope0 < 0 || slope1 < 0)
                    NegativeSlope(i, j, heightMap);
                else if (slope0 > 0 && slope1 < 0)
                    LocalMaxima(i, j, heightMap);
                else if (sandAccumulate > 0)
                    heightMap[i, j] += DepositSand(i, j, heightMap[i, j], heightMap[i, j + 1]);
                //else if (slope0 > 0 && slope1 > 0)
                //     indexPositive.Add(i);
            }
        }
    }

    // private void PositiveSlope(int point0)
    // {
    //     float slope0 = Algebra.Slope(vertices[point0 - 1], vertices[point0]);
    //     float slope1 = Algebra.Slope(vertices[point0], vertices[point0 + 1]);
    //     if (Mathf.Abs(slope1 - slope0) > deltaSlopeWind)
    //     {
    //         float diff = vertices[point0].y - vertices[point0 + 1].y;
    //         vertices[point0].y -= Mathf.Min(diff / 4f, deltaHeightWind);
    //         vertices[point0 + 1].y += Mathf.Min(diff / 4f, deltaHeightWind);
    //     }
    // }

    private static void NegativeSlope(int i, int j, float[,] heightMap)
    {
        float height0 = heightMap[i, j];
        float height1 = heightMap[i, j + 1];
        // Vector3 dir = new Vector3(i, height1, j + 1) - new Vector3(i, height0, j);
        Vector3 dir = new Vector3(0, height1, 1) - new Vector3(0, height0, 0);
        float angle = Vector3.Angle(Vector3.forward, dir);

        if (Mathf.Abs(angle) > angleSleep)
        {
            float diff = Mathf.Abs(height0 - height1);
            float valueErosion = Mathf.Min(diff / 2f, deltaGravity);
            heightMap[i, j] -= valueErosion;
            heightMap[i, j + 1] += valueErosion;
            sandAccumulate += valueErosion;
        }
        else if (sandAccumulate > 0)
            heightMap[i, j] += DepositSand(i, j, height0, height1);
    }

    private static void LocalMaxima(int i, int j, float[,] heightMap)
    {
        float height0 = heightMap[i, j - 1];
        float height1 = heightMap[i, j];
        float height2 = heightMap[i, j + 1];
        Vector3 dir0 = new Vector3(0, height1, 1) - new Vector3(0, height0, 0);
        Vector3 dir1 = new Vector3(0, height2, 2) - new Vector3(0, height1, 1);
        float angle0 = Vector3.Angle(Vector3.forward, dir0);
        float angle1 = Vector3.Angle(Vector3.forward, dir1);

        if ((Mathf.Abs(angle0) + Mathf.Abs(angle1)) > peekAngle)
        {
            float diff = Mathf.Abs(height1 - height2);
            float valueErosion = Mathf.Min(diff / 2f,  deltaGravity);
            heightMap[i, j] -= valueErosion;
            heightMap[i, j + 1] += valueErosion;
            sandAccumulate += valueErosion;
        }
        else if (sandAccumulate > 0)
            heightMap[i, j] += DepositSand(i, j, height0, height1);
    }

    private static float DepositSand(int i, int j, float height0, float height1)
    {
        Vector3 dir = new Vector3(i, height1, j + 1) - new Vector3(i, height0, j);
        float angle = Vector3.Angle(Vector3.forward, dir);

        float diff = Mathf.Abs(height0 - height1);
        float valueSoil = Mathf.Min(Mathf.Cos(angle * Mathf.Deg2Rad) * deltaGravity, diff / 2f);
        if (valueSoil > sandAccumulate)
        {
            float valueReturn = sandAccumulate;
            sandAccumulate = 0;
            return valueReturn;
        }
        else
        {
            sandAccumulate -= valueSoil;
            return valueSoil;
        }
    }
}
