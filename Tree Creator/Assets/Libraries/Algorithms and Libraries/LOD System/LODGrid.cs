using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LODGrid : MonoBehaviour
{
    [Header("Map Settings")]
    [Header("Usar 241, 181, 121, 61")]
    public int resolution = 121;

    [Range(0, 6)]
    public int levelOfDetail = 1;
    public float frequencyCellular;
    public float amplitudeCellular;
    public float frequencyPerlin;
    public float amplitudePerlin;
    public int quantityMaps;


    public Material material;
    private float[,] heightMap;
    Mesh[] meshes;
    Transform[] containers;


    void Start()
    {
        heightMap = new float[resolution * quantityMaps, resolution * quantityMaps];
        HeightMapFiller.FillHeightMapCellular(heightMap, frequencyCellular, amplitudeCellular, 1);
        HeightMapFiller.FillHeightMapPerlin(heightMap, frequencyPerlin, amplitudePerlin);
        meshes = HeightMapToMesh.GenerateMeshes(heightMap, resolution, 1, levelOfDetail);
        CreateContainers();
    }

    void CreateContainers()
    {
        containers = new Transform[quantityMaps * quantityMaps];
        for (int i = 0; i < quantityMaps * quantityMaps; i++)
        {
            containers[i] = new GameObject("Terrain").transform;
            containers[i].SetParent(transform);
            containers[i].gameObject.AddComponent<MeshFilter>().mesh = meshes[i];
            containers[i].gameObject.AddComponent<MeshRenderer>().material = material;
        }

        for (int y = 0, n = 0; y < quantityMaps; y++)
            for (int x = 0; x < quantityMaps; x++)
                containers[n++].position = new Vector3((resolution - 1) * y, 0, (resolution - 1) * x);
    }
}
