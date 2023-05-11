using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class DuneGenerator : MonoBehaviour
{
    public int seed;
    public int resolution;
    public float frequency;
    public float amplitude;

    private float[,] heightMap;
    private Vector3[] vertices;
    private Mesh mesh;


    [Header("Dune Settings")]
    public bool erosion;
    public int iterations = 1;
    // public float deltaSlopeWind = 0.05f;
    // public float deltaHeightWind = 0.09f;
    public float angleSleep = 30f;
    public float deltaGravity = 0.01f;

    private float sandAccumulate;

    public bool staticErosion;

    void Start()
    {
        mesh = new Mesh();
        mesh.name = "Dunes";
        // vertices = new Vector3[resolution * resolution];
        heightMap = new float[resolution, resolution];
        HeightMapFiller.FillHeightMapPerlin(heightMap, frequency, amplitude, seed);
        // HeightMapToMesh.Generate(heightMap, mesh, vertices, 1);
        // vertices = mesh.vertices;
        if (erosion)
        {
            if (staticErosion)
            {
                DuneErosion.Erosion(heightMap, iterations, angleSleep, deltaGravity);
                mesh = HeightMapToMesh.Generate(heightMap, 1);
            }
            else
            {
                HeightMapToMesh.Generate(heightMap, mesh, vertices, 1);
                vertices = mesh.vertices;
                for (int i = 0; i < iterations; i++)
                    ErosionDunes();
                mesh.vertices = vertices;
                mesh.RecalculateNormals();
            }
        }
        // HeightMapToMesh.Generate(heightMap, 1);
        // mesh.vertices = vertices;
        // mesh.RecalculateNormals();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    private void ApplyAmplitude()
    {
        for (int i = 0; i < resolution; i++)
            for (int j = 0; j < resolution; j++)
                heightMap[i, j] *= amplitude;
    }

    private void ErosionDunes()
    {
        int k = 0;
        for (int i = 0; i < resolution; i++)
        {
            k = 1 + (i * resolution);
            for (int j = 1; j < resolution - 1; j++, k++)
            {
                float slope0 = Algebra.SlopeYZ(vertices[k - 1], vertices[k]);
                float slope1 = Algebra.SlopeYZ(vertices[k], vertices[k + 1]);
                if (slope0 < 0 || slope1 < 0)
                    NegativeSlope(k);
                else if (sandAccumulate > 0)
                    DepositSand(k);
                //else if (slope0 > 0 && slope1 > 0)
                //     indexPositive.Add(i);

            }
        }
        // foreach (var item in indexPositive)
        //     PositiveSlope(item);
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

    private void NegativeSlope(int point0)
    {
        Vector3 dir = vertices[point0 + 1] - vertices[point0];
        float angle = Vector3.Angle(Vector3.forward, dir);
        if (Mathf.Abs(angle) > angleSleep)
        {
            float diff = Mathf.Abs(vertices[point0].y - vertices[point0 + 1].y);
            float valueErosion = Mathf.Min(diff / 2f, deltaGravity);
            vertices[point0].y -= valueErosion;
            vertices[point0 + 1].y += valueErosion;
            sandAccumulate += valueErosion;
        }
        else if (sandAccumulate > 0)
            DepositSand(point0);
    }

    private void DepositSand(int point0)
    {
        Vector3 dir = vertices[point0 + 1] - vertices[point0];
        float angle = Vector3.Angle(Vector3.forward, dir);

        float diff = Mathf.Abs(vertices[point0].y - vertices[point0 + 1].y);
        float valueSoil = Mathf.Min(Mathf.Cos(angle * Mathf.Deg2Rad) * deltaGravity, diff / 2f);
        if (valueSoil > sandAccumulate)
        {
            vertices[point0].y += sandAccumulate;
            sandAccumulate = 0;
        }
        else
        {
            vertices[point0].y += valueSoil;
            sandAccumulate -= valueSoil;
        }
    }
}

public static class Direction
{
    public static readonly Vector2Int North = new Vector2Int(0, 1);
    public static readonly Vector2Int South = new Vector2Int(0, -1);
    public static readonly Vector2Int East = new Vector2Int(1, 0);
    public static readonly Vector2Int West = new Vector2Int(-1, 0);


    public static readonly Vector2Int NorthEast = new Vector2Int(1, 1);
    public static readonly Vector2Int NorthWeast = new Vector2Int(-1, 1);
    public static readonly Vector2Int SouthEast = new Vector2Int(1, -1);
    public static readonly Vector2Int SouthWeast = new Vector2Int(-1, -1);
}