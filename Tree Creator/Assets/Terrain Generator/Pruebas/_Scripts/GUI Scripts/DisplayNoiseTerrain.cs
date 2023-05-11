using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DisplayNoiseTerrain : MonoBehaviour
{
    [Header("General")]
    [Tooltip("Potencias de 2")]
    public int resolucion = 512;
    public int seed = 1;
    public float frequency = 0.01f;
    public float amplitude = 1f;
    public FastNoiseLite.NoiseType TipoDeRuido = FastNoiseLite.NoiseType.OpenSimplex2;


    [Header("Sample Position")]
    public int posX = 0;
    public int posY = 0;


    [Header("Fractals")]
    public FastNoiseLite.FractalType fractalType = FastNoiseLite.FractalType.None;
    public int octaves = 3;
    public float fractalWeightedStrength = 0;
    public float fractalGain = 0.5f;
    public float fractalLacunarity = 2;


    [Header("Cellular(Voronoi) Noise")]
    public FastNoiseLite.CellularReturnType cellularType = FastNoiseLite.CellularReturnType.CellValue;
    public FastNoiseLite.CellularDistanceFunction cellularDistance = FastNoiseLite.CellularDistanceFunction.EuclideanSq;
    public float cellularJitter = 1;


    [Header("Domain Warp Domain")]
    public FastNoiseLite.DomainWarpType domainWarpType = FastNoiseLite.DomainWarpType.OpenSimplex2;
    public float domainWarpAmp = 1;

    [Header("Fractal Pin Pong")]
    public float fractalPingPongStrengh = 2;


    private Color[] colores;
    private FastNoiseLite fastNoise;
    private Texture2D texture;

    private float prevFrequency, prevWeightedStrength, prevGain, prevLacunarity,
                prevJitter, prevWarpAmp, prevPingPong;

    private int prevOctaves, prevX, prevY;

    FastNoiseLite.NoiseType prevNoise;
    FastNoiseLite.FractalType prevFractal;
    FastNoiseLite.CellularReturnType prevCellularReturnType;
    FastNoiseLite.CellularDistanceFunction prevCellularDistance;
    FastNoiseLite.DomainWarpType prevDomainWarp;

    [HideInInspector]
    public bool change;
    public float[] values;

    private MeshFilter meshFilter;
    private Mesh meshMap;
    private float[,] heightMap;

    private Vector3[] vertices;

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshMap = new Mesh();
        meshMap.name = "Map";
        heightMap = new float[resolucion, resolucion];
        vertices = new Vector3[resolucion * resolucion];
        HeightMapToMesh.Generate(heightMap, meshMap, vertices, 1);
        meshFilter.mesh = meshMap;
    }

    void Start()
    {
        // texture = new Texture2D(resolucion, resolucion);
        fastNoise = new FastNoiseLite(seed);
        UpdateNoise();
        UpdateNoiseParameters();

        // texture.wrapMode = TextureWrapMode.Clamp;
        // texture.filterMode = FilterMode.Point;
        // GetComponent<MeshRenderer>().material.mainTexture = texture;
        ActualizarDatos();
        ActualizarMapa();
        // UpdateValue();
    }

    private void OnValidate()
    {
        UpdateNoise();
        UpdateNoiseParameters();

        if (change)
        {
            ActualizarMapa();
            // UpdateValue();
            change = false;
        }
    }

    // void Update()
    // {
    //     UpdateNoise();
    //     UpdateNoiseParameters();

    //     if (change)
    //     {
    //         ActualizarColores();
    //         // UpdateValue();
    //         // OnChange();
    //         change = false;
    //     }
    // }

    // private void UpdateValue()
    // {
    //     for (int y = 0, n = 0; y < resolucion; y++)
    //         for (int x = 0; x < resolucion; x++)
    //             values[n++] = fastNoise.GetNoise(x + posX, y + posY);
    // }

    public float GetValue(int x, int y)
    {
        return values[x + (resolucion * y)];
    }

    private void ActualizarMapa()
    {
        // float stepGrad = 1f / (float)((float)(resolucion * 0.5f) - 1f);
        for (int y = 0; y < resolucion; y++)
        {
            for (int x = 0; x < resolucion; x++)
            {
                // float value = fastNoise.GetNoise(x + posX, y + posY) * 0.5f + 0.5f;
                heightMap[y, x] = fastNoise.GetNoise(x + posX, y + posY);
                // float finalValue = Mathf.Clamp01(value - (y * step));
                // float value = fastNoise.GetNoise(x + posX, y + posY) + 1f - (y * stepGrad);
                // float finalValue = value >= 0 ? 1 : 0;
            }
        }
        UpdateVertices();
        meshMap.vertices = vertices;
        meshMap.RecalculateNormals();
    }

    private void UpdateVertices()
    {
        for (int y = 0, n = 0; y < resolucion; y++)
        {
            for (int x = 0; x < resolucion; x++)
            {
                vertices[n] = new Vector3(y, heightMap[y, x] * amplitude, x);
                n++;
            }
        }
    }

    private void UpdateNoise()
    {
        if (prevNoise != TipoDeRuido)
        {
            fastNoise.SetNoiseType(TipoDeRuido);
            prevNoise = TipoDeRuido;
            change = true;
        }
        else if (prevCellularReturnType != cellularType)
        {
            fastNoise.SetCellularReturnType(cellularType);
            prevCellularReturnType = cellularType;
            change = true;
        }
        else if (prevCellularDistance != cellularDistance)
        {
            fastNoise.SetCellularDistanceFunction(cellularDistance);
            prevCellularDistance = cellularDistance;
            change = true;
        }
        else if (prevFractal != fractalType)
        {
            fastNoise.SetFractalType(fractalType);
            prevFractal = fractalType;
            change = true;
        }
        else if (prevDomainWarp != domainWarpType)
        {
            fastNoise.SetDomainWarpType(domainWarpType);
            prevDomainWarp = domainWarpType;
            change = true;
        }
    }

    private void UpdateNoiseParameters()
    {
        if (prevFrequency != frequency)
        {
            fastNoise.SetFrequency(frequency);
            prevFrequency = frequency;
            change = true;
        }
        else if (prevOctaves != octaves)
        {
            fastNoise.SetFractalOctaves(octaves);
            prevOctaves = octaves;
            change = true;
        }
        else if (prevWeightedStrength != fractalWeightedStrength)
        {
            fastNoise.SetFractalWeightedStrength(fractalWeightedStrength);
            prevWeightedStrength = fractalWeightedStrength;
            change = true;
        }
        else if (prevJitter != cellularJitter)
        {
            fastNoise.SetCellularJitter(cellularJitter);
            prevJitter = cellularJitter;
            change = true;
        }
        else if (prevWarpAmp != domainWarpAmp)
        {
            fastNoise.SetDomainWarpAmp(domainWarpAmp);
            prevWarpAmp = domainWarpAmp;
            change = true;
        }
        else if (prevGain != fractalGain)
        {
            fastNoise.SetFractalGain(fractalGain);
            prevGain = fractalGain;
            change = true;
        }
        else if (prevLacunarity != fractalLacunarity)
        {
            fastNoise.SetFractalLacunarity(fractalLacunarity);
            prevLacunarity = fractalLacunarity;
            change = true;
        }
        else if (prevPingPong != fractalPingPongStrengh)
        {
            fastNoise.SetFractalPingPongStrength(fractalPingPongStrengh);
            prevPingPong = fractalPingPongStrengh;
            change = true;
        }
        else if (prevX != posX || prevY != posY)
        {
            prevX = posX;
            prevY = posY;
            change = true;
        }
    }

    private void ActualizarDatos()
    {
        prevFrequency = frequency;
        prevOctaves = octaves;
        prevWeightedStrength = fractalWeightedStrength;
        prevJitter = cellularJitter;
        prevWarpAmp = domainWarpAmp;
        prevGain = fractalGain;
        prevLacunarity = fractalLacunarity;
        prevPingPong = fractalPingPongStrengh;
        prevX = posX;
        prevY = posY;

        prevNoise = TipoDeRuido;
        prevFractal = fractalType;
        prevCellularReturnType = cellularType;
        prevCellularDistance = cellularDistance;
        prevDomainWarp = domainWarpType;
    }
}
