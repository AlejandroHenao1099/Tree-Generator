using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LineDune : MonoBehaviour
{
    [Header("Noise Settings")]
    public float size, amplitude, frequency;
    public float minSlope;
    public int resolution, seed;
    private int prevResolution;
    Vector3[] points;
    List<int> indexToChange = new List<int>();
    List<int> indexPositive = new List<int>();
    List<int> indexNegative = new List<int>();
    private FastNoiseLite fastNoiseLite;

    [Header("Dune Settings")]
    public bool erosion;
    public int iterations = 1;
    public float deltaSlopeWind = 0.05f;
    public float deltaHeightWind = 0.09f;
    public float angleSleep = 30f;
    public float deltaGravity = 0.01f;

    public float sizeCube;

    private void Initialize()
    {
        // if (resolution != prevResolution)
        // {
        points = new Vector3[resolution];
        //     prevResolution = resolution;
        // }
        float step = size / (float)resolution;
        // float stepAngle = (2f * Mathf.PI) / (float)resolution;

        for (int i = 0; i < resolution; i++)
            points[i] = new Vector3(i * step, fastNoiseLite.GetNoise(i * step, 0) * amplitude, 0);
        // points[i] = new Vector3(i * step, Mathf.Sin((i * stepAngle) * frequency) * amplitude, 0);
    }

    private void Start()
    {
        fastNoiseLite = new FastNoiseLite(seed);
        fastNoiseLite.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
        fastNoiseLite.SetFrequency(frequency);
        Initialize();
        if (erosion)
            for (int i = 0; i < iterations; i++)
                ErosionDunes();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        for (int i = 0; i < resolution - 1; i++)
            Gizmos.DrawLine(points[i], points[i + 1]);
        Gizmos.color = Color.green;
        for (int i = 0; i < resolution; i++)
            Gizmos.DrawCube(points[i], Vector3.one * sizeCube);
    }

    private void SearchPoints()
    {
        float currSlope = 0;
        for (int j = 1; j < resolution; j++)
        {
            currSlope = Algebra.Slope(points[j - 1], points[j]);
            if (currSlope < minSlope)
                indexToChange.Add(j);
        }
        ApplyChange();
        indexToChange.Clear();
    }

    private void ApplyChange()
    {
        int startIndex = -1;
        int endIndex = -1;
        for (int i = 0; i < indexToChange.Count - 1; i++)
        {
            if (startIndex == -1)
            {
                startIndex = indexToChange[i];
                continue;
            }
            if (indexToChange[i] + 1 != indexToChange[i + 1] || (i + 1 == indexToChange.Count - 1))
                endIndex = indexToChange[i];
            if (endIndex != -1)
            {
                Vector3 startPoint = points[startIndex];
                Vector3 endPoint = points[endIndex];
                Vector3 controlStart = Vector3.Lerp(startPoint, endPoint, 0.1f);
                Vector3 controlEnd = Vector3.Lerp(startPoint, endPoint, 0.9f);
                controlStart.y -= 0.8f * (amplitude * 2);
                controlEnd.y -= 0.1f * (amplitude * 2);
                float slope = Algebra.SlopeYZ(startPoint, endPoint);
                for (int j = startIndex; j <= endIndex; j++)
                {
                    float y = Algebra.RectPointSlope(points[j].x, startPoint, slope);
                    // float t = Mathf.InverseLerp(startPoint.x, endPoint.x, points[j].x);
                    points[j].y = y;
                    // points[j] = BezierCurve.Create(t, startPoint, controlStart, controlEnd, endPoint);
                }
                startIndex = -1;
                endIndex = -1;
            }
        }
    }

    private void ErosionDunes()
    {
        for (int i = 1; i < points.Length - 1; i++)
        {
            float slope0 = Algebra.Slope(points[i - 1], points[i]);
            float slope1 = Algebra.Slope(points[i], points[i + 1]);
            // if (slope0 > 0 && slope1 > 0)
            //     indexPositive.Add(i);
            if (slope0 < 0 || slope1 < 0)
                NegativeSlope(i);
            // indexNegative.Add(i);
            // if (slope0 > 0 && slope1 < 0)
            //     continue;

        }
        // foreach (var item in indexPositive)
        //     PositiveSlope(item);
        // foreach (var item in indexNegative)
        //     NegativeSlope(item);
    }

    private void PositiveSlope(int point0)
    {
        float slope0 = Algebra.Slope(points[point0 - 1], points[point0]);
        float slope1 = Algebra.Slope(points[point0], points[point0 + 1]);
        if (Mathf.Abs(slope1 - slope0) > deltaSlopeWind)
        {
            float diff = points[point0].y - points[point0 + 1].y;
            points[point0].y -= Mathf.Min(diff / 4f, deltaHeightWind);
            points[point0 + 1].y += Mathf.Min(diff / 4f, deltaHeightWind);
            // points[point0].y -= deltaHeightWind;
            // points[point0 + 1].y += deltaHeightWind;
        }
    }

    private void NegativeSlope(int point0)
    {
        Vector3 dir = points[point0 + 1] - points[point0];
        float angle = Vector3.Angle(Vector3.right, dir);

        if (Mathf.Abs(angle) > angleSleep)
        {
            float diff = points[point0].y - points[point0 + 1].y;
            points[point0].y -= Mathf.Min(diff / 2f, deltaGravity);
            points[point0 + 1].y += Mathf.Min(diff / 2f, deltaGravity);
        }
    }

}
