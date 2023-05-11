using UnityEngine;
using System;

[Serializable]
public class NoiseData
{
    [Header("General")]
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


    private FastNoiseLite fastNoise;

    private float prevFrequency, prevWeightedStrength, prevGain, prevLacunarity,
                prevJitter, prevWarpAmp, prevPingPong;

    private int prevOctaves, prevX, prevY;

    FastNoiseLite.NoiseType prevNoise;
    FastNoiseLite.FractalType prevFractal;
    FastNoiseLite.CellularReturnType prevCellularReturnType;
    FastNoiseLite.CellularDistanceFunction prevCellularDistance;
    FastNoiseLite.DomainWarpType prevDomainWarp;

    public void Initizialize()
    {
        fastNoise = new FastNoiseLite(seed);
        Update();
    }

    public float GetValue(int x, int y)
    {
        switch (fractalType)
        {
            case FastNoiseLite.FractalType.DomainWarpIndependent:
            case FastNoiseLite.FractalType.DomainWarpProgressive:
                float xWarp = x + posX;
                float yWarp = y + posY;
                fastNoise.DomainWarp(ref xWarp ,ref yWarp);
                return fastNoise.GetNoise(xWarp, yWarp) * amplitude;
            default:
                return fastNoise.GetNoise(x + posX, y + posY) * amplitude;
        }
    }

    public void GetValueWarp(float x, float y)
    {
        fastNoise.DomainWarp(ref x, ref y);
    }

    public void Update()
    {
        UpdateNoise();
        UpdateNoiseParameters();
    }

    private void UpdateNoise()
    {
        if (prevNoise != TipoDeRuido)
        {
            fastNoise.SetNoiseType(TipoDeRuido);
            prevNoise = TipoDeRuido;
        }
        else if (prevCellularReturnType != cellularType)
        {
            fastNoise.SetCellularReturnType(cellularType);
            prevCellularReturnType = cellularType;
        }
        else if (prevCellularDistance != cellularDistance)
        {
            fastNoise.SetCellularDistanceFunction(cellularDistance);
            prevCellularDistance = cellularDistance;
        }
        else if (prevFractal != fractalType)
        {
            fastNoise.SetFractalType(fractalType);
            prevFractal = fractalType;
        }
        else if (prevDomainWarp != domainWarpType)
        {
            fastNoise.SetDomainWarpType(domainWarpType);
            prevDomainWarp = domainWarpType;
        }
    }

    private void UpdateNoiseParameters()
    {
        if (prevFrequency != frequency)
        {
            fastNoise.SetFrequency(frequency);
            prevFrequency = frequency;
        }
        else if (prevOctaves != octaves)
        {
            fastNoise.SetFractalOctaves(octaves);
            prevOctaves = octaves;
        }
        else if (prevWeightedStrength != fractalWeightedStrength)
        {
            fastNoise.SetFractalWeightedStrength(fractalWeightedStrength);
            prevWeightedStrength = fractalWeightedStrength;
        }
        else if (prevJitter != cellularJitter)
        {
            fastNoise.SetCellularJitter(cellularJitter);
            prevJitter = cellularJitter;
        }
        else if (prevWarpAmp != domainWarpAmp)
        {
            fastNoise.SetDomainWarpAmp(domainWarpAmp);
            prevWarpAmp = domainWarpAmp;
        }
        else if (prevGain != fractalGain)
        {
            fastNoise.SetFractalGain(fractalGain);
            prevGain = fractalGain;
        }
        else if (prevLacunarity != fractalLacunarity)
        {
            fastNoise.SetFractalLacunarity(fractalLacunarity);
            prevLacunarity = fractalLacunarity;
        }
        else if (prevPingPong != fractalPingPongStrengh)
        {
            fastNoise.SetFractalPingPongStrength(fractalPingPongStrengh);
            prevPingPong = fractalPingPongStrengh;
        }
        else if (prevX != posX || prevY != posY)
        {
            prevX = posX;
            prevY = posY;
        }
    }
}
