using UnityEngine;

public class BigDesert : MonoBehaviour
{
    [Header("Map Settings")]
    public int resolution = 128;
    public int levelOfDetail = 1;
    public float frequencyCellular;
    public float amplitudeCellular;
    public float frequencyPerlin;
    public float amplitudePerlin;
    public int quantityMaps;


    [Header("Erosion Settings")]
    public bool erosion;
    public int iterations = 1;
    public float angleSleep = 30f;
    public float peekAngle = 40f;
    public float deltaGravity = 0.01f;

    public Material material;
    private float[,] heightMap;
    Mesh[] meshes;
    Transform[] containers;


    void Start()
    {
        heightMap = new float[resolution * quantityMaps, resolution * quantityMaps];
        HeightMapFiller.FillHeightMapCellular(heightMap, frequencyCellular, amplitudeCellular, 1);
        HeightMapFiller.FillHeightMapPerlin(heightMap, frequencyPerlin, amplitudePerlin);
        if(erosion)
            DuneErosion.Erosion(heightMap, iterations, angleSleep, deltaGravity, peekAngle);
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
