public static class HeightMapFiller
{
    public static void FillHeightMapPerlin(float[,] heightMap, float frequency, float amplitude, int seed = 1337)
    {
        FastNoiseLite fastNoise = new FastNoiseLite(seed);
        fastNoise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
        fastNoise.SetFrequency(frequency);

        int sizeMap = heightMap.GetLength(0);
        for (int i = 0; i < sizeMap; i++)
            for (int j = 0; j < sizeMap; j++)
                heightMap[i, j] += fastNoise.GetNoise(j, i) * amplitude;

    }

    public static void FillHeightMapCellular(float[,] heightMap, float frequency, float amplitude, float jitter,
        FastNoiseLite.CellularDistanceFunction cellularDistanceFunction = FastNoiseLite.CellularDistanceFunction.EuclideanSq,
        FastNoiseLite.CellularReturnType cellularReturnType = FastNoiseLite.CellularReturnType.Distance)
    {
        FastNoiseLite fastNoise = new FastNoiseLite();
        fastNoise.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
        fastNoise.SetCellularDistanceFunction(cellularDistanceFunction);
        fastNoise.SetCellularReturnType(cellularReturnType);
        fastNoise.SetCellularJitter(jitter);
        fastNoise.SetFrequency(frequency);

        int sizeMap = heightMap.GetLength(0);
        for (int i = 0; i < sizeMap; i++)
            for (int j = 0; j < sizeMap; j++)
                heightMap[i, j] += fastNoise.GetNoise(j, i) * amplitude;
    }

}
