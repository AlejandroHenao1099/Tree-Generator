using UnityEngine;
using Terrain;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class CreateTerrain : MonoBehaviour
{
    [Header("Terrain settings")]
    public Material materialTerrain;

    [Range(4, 128), Tooltip("Usa potencias de 2")]
    public int resolutionTerrain = 64;
    public int scaleTerrain = 1;


    [Header("Noise Settings")]
    public int octaves = 1;
    public float lacunarity = 1.98f;

    public float frequency = 0.026f;
    public float amplitude = 3;


    [Header("Diamond Square Settings")]
    public float roughness = 1;
    public float seed = 10;

    public float h;


    private float[,] heightMap;
    public bool hetero = true;
    public float offset = 1;

    void Start()
    {
        heightMap = new float[resolutionTerrain + 1, resolutionTerrain + 1];
        // CreateHeightMapPerlinNoise();
        CreateHeightMapDiamondSquare();
        if (hetero)
            CreateHeightMapHeteroTerrain();
        var mesh = HeightMapToMesh.Generate(heightMap, scaleTerrain);
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshRenderer>().material = materialTerrain;
    }

    private void CreateHeightMapDiamondSquare()
    {
        heightMap = DiamondSquare.GetData(resolutionTerrain, roughness, seed);
    }

    private void CreateHeightMapPerlinNoise()
    {
        AddOctave(1, amplitude);
        float h = amplitude * 0.5f;
        for (int Oct = 1; Oct < octaves; Oct++, h *= 0.5f)
            AddOctave(lacunarity, h);
    }

    private void AddOctave(float lacunarity, float amplitude)
    {
        for (int i = 0; i < resolutionTerrain + 1; i++)
            for (int j = 0; j < resolutionTerrain + 1; j++)
                heightMap[i, j] += UnityPerlinNoise.GetNoise(new Vector2(i, j), frequency * lacunarity, amplitude);
    }


    private bool first = true;
    private float[] exponent_array;


    private void CreateHeightMapHeteroTerrain()
    {
        for (int i = 0; i < resolutionTerrain + 1; i++)
            for (int j = 0; j < resolutionTerrain + 1; j++)
                heightMap[i, j] += HeteroTerrain(new Vector3(i, heightMap[i, j], j), h, lacunarity, octaves, offset);
    }

    private float HeteroTerrain(Vector3 point, float H, float lacunarity, int octaves, float offset)
    {
        float value;
        float increment;
        float frequency;
        // float remainder;
        int i;
        if (first)
        {
            exponent_array = new float[octaves + 1];
            frequency = 1f;
            for (i = 0; i <= octaves; i++)
            {
                exponent_array[i] = Mathf.Pow(frequency, -H);
                frequency *= lacunarity;
            }
            first = false;
        }
        value = offset + UnityPerlinNoise.GetNoise(point);
        point *= lacunarity;

        for (i = 1; i < octaves; i++)
        {
            /* obtain displaced noise value */
            increment = UnityPerlinNoise.GetNoise(point) + offset;
            /* scale amplitude appropriately for this frequency */
            increment *= exponent_array[i];
            /* scale increment by current “altitude” of function */
            increment *= value;
            /* add increment to “value” */
            value += increment;
            /* raise spatial frequency */
            point *= lacunarity;
        }
        increment = (UnityPerlinNoise.GetNoise(point) + offset) * exponent_array[i];
        value += 1 * increment * value;
        return value;
    }


}
