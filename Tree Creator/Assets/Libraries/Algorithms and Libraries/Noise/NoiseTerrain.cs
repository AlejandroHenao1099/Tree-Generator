using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Terrain;

public class NoiseTerrain : MonoBehaviour
{
    public int resolution = 255;
    public NoiseData[] noiseDatas;
    public TypeOfErosion typeOfErosion;
    public bool applyErosion;

    public int iterationsErosion;
    public KernelType kernelType;
    public HydraulicErosionData hydraulicErosionData;


    private float[,] heightMap;
    private MeshFilter meshFilter;
    private Mesh meshMap;
    private Vector3[] vertices;

    private bool onPlay = false;

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshMap = new Mesh();
        meshMap.name = "Map";
        heightMap = new float[resolution, resolution];
        vertices = new Vector3[resolution * resolution];
        HeightMapToMesh.Generate(heightMap, meshMap, vertices, 1);
        meshFilter.mesh = meshMap;
    }

    void Start()
    {
        foreach (var noise in noiseDatas)
            noise.Initizialize();

        UpdateNoise();
        onPlay = true;
    }

    private void OnValidate()
    {
        if (onPlay)
            UpdateNoise();
    }

    private void UpdateNoise()
    {
        foreach (var noise in noiseDatas)
            noise.Update();
        UpdateMap();
    }

    private void UpdateMap()
    {
        for (int i = 0; i < resolution; i++)
            for (int j = 0; j < resolution; j++)
            {
                heightMap[i, j] = 0;
                foreach (var noise in noiseDatas)
                    heightMap[i, j] += noise.GetValue(j, i);
            }
        ApplyErosion();
    }

    private void ApplyErosion()
    {
        if (applyErosion)
        {
            switch (typeOfErosion)
            {
                case TypeOfErosion.Thermal:
                    ThermalErosion.ApplyErosion(heightMap, kernelType, iterationsErosion);
                    break;
                case TypeOfErosion.Hydraulic:
                    HydraulicErosion.ApplyErosion(heightMap, iterationsErosion, kernelType,
                        hydraulicErosionData.rain, hydraulicErosionData.solubility, hydraulicErosionData.evaporation,
                             hydraulicErosionData.capacity);
                    break;
                case TypeOfErosion.Fast:
                    FastErosion.ApplyErosion(heightMap, iterationsErosion, kernelType);
                    break;
                default:
                    break;
            }
            applyErosion = false;
        }
        UpdateVertices();
    }

    private void UpdateVertices()
    {
        for (int y = 0, n = 0; y < resolution; y++)
            for (int x = 0; x < resolution; x++)
                vertices[n++] = new Vector3(y, heightMap[y, x], x);
        meshMap.vertices = vertices;
        meshMap.RecalculateNormals();
    }
}


public enum TypeOfErosion
{
    Thermal, Hydraulic, Fast
}

[System.Serializable]
public struct HydraulicErosionData
{
    public float rain;
    public float solubility;
    public float evaporation;
    public float capacity;
}