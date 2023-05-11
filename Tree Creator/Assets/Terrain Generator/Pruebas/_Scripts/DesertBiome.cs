﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesertBiome : MonoBehaviour
{
    [Header("General")]
    [Tooltip("Potencias de 2")]
    public int resolucion = 512;
    public int seed = 1;
    public float frequency = 0.01f;
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


    [Header("Fractal Pin Pong")]
    public float fractalPingPongStrengh = 2;


    private Color[] colores;
    private FastNoiseLite fastNoise;
    private Texture2D texture;

    private float prevFrequency, prevWeightedStrength, prevGain, prevLacunarity,
                prevJitter, prevPingPong;

    private int prevOctaves, prevX, prevY;

    FastNoiseLite.NoiseType prevNoise;
    FastNoiseLite.FractalType prevFractal;
    FastNoiseLite.CellularReturnType prevCellularReturnType;
    FastNoiseLite.CellularDistanceFunction prevCellularDistance;

    private bool change;
    private float[,] heightMap;
    public int grids = 8;
    public float distanceSqrDeath = 16f;


    void Start()
    {
        texture = new Texture2D(resolucion, resolucion);
        heightMap = new float[resolucion, resolucion];
        fastNoise = new FastNoiseLite(seed);
        UpdateNoise();
        UpdateNoiseParameters();

        colores = new Color[resolucion * resolucion];
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Point;
        GetComponent<MeshRenderer>().material.mainTexture = texture;
        ActualizarDatos();
        ActualizarColores();
    }

    private void OnValidate()
    {
        UpdateNoise();
        UpdateNoiseParameters();

        if (change)
        {
            ActualizarColores();
            change = false;
        }
    }

    private void ActualizarColores()
    {
        for (int y = 0, n = 0; y < resolucion; y++)
        {
            for (int x = 0; x < resolucion; x++)
            {
                float value = fastNoise.GetNoise(x + posX, y + posY) * 0.5f + 0.5f;
                heightMap[y, x] = value;
                colores[n++] = new Color(value, value, value, 1);
            }
        }
        SearchMaxLocal();
        texture.SetPixels(0, 0, resolucion, resolucion, colores);
        texture.Apply();
    }

    private void SearchMaxLocal()
    {
        List<int> maximas = new List<int>();
        List<Vector2Int> posMaximas = new List<Vector2Int>();
        int sizeGrid = resolucion / grids;

        for (int x = 0; x < grids; x++)
        {
            for (int y = 0; y < grids; y++)
            {
                int n = 0;
                Vector2Int pos = Vector2Int.zero;
                float maxNumber = float.MinValue;
                for (int i = x * sizeGrid; i < (x * sizeGrid) + sizeGrid; i++)
                {
                    for (int j = y * sizeGrid; j < (y * sizeGrid) + sizeGrid; j++)
                    {
                        if (heightMap[i, j] > maxNumber)
                        {
                            maxNumber = heightMap[i, j];
                            pos.x = j;
                            pos.y= i;
                            n = (resolucion * i) + j;
                        }
                    }
                }
                maximas.Add(n);
                posMaximas.Add(pos);
            }
        }
        PaintMaxLocal(maximas);
    }

    // private void CheckValues(List<int> maximas, List<Vector2Int> posMaximas)
    // {

    //     for (int i = 0; i < posMaximas.Count; )
    //     {
    //         for (int j = i + 1; j < posMaximas.Count; j++)
    //         {
    //             if(posMaximas[i] - posMaximas[j] )
    //         }
    //     }
    // }

    // private void ApplyTransition(List<Vector2Int> posMaximas)
    // {
    //     foreach (var p in posMaximas)
    //     {
            
    //         heightMap[p.y, p.x] = 0;
    //     }
    // }


    private void PaintMaxLocal(List<int> maximas)
    {
        foreach (var item in maximas)
            colores[item] = Color.red;
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
        prevGain = fractalGain;
        prevLacunarity = fractalLacunarity;
        prevPingPong = fractalPingPongStrengh;
        prevX = posX;
        prevY = posY;

        prevNoise = TipoDeRuido;
        prevFractal = fractalType;
        prevCellularReturnType = cellularType;
        prevCellularDistance = cellularDistance;
    }

}
