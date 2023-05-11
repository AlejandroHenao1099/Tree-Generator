using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseTexture : MonoBehaviour
{
    public int resolution = 255;
    public NoiseData[] noiseDatas;

    private Color[] colores;
    private Texture2D texture;
    private float[,] heightMap;

    private bool onPlay = false;

    void Start()
    {
        heightMap = new float[resolution, resolution];
        texture = new Texture2D(resolution, resolution);
        colores = new Color[resolution * resolution];
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Point;

        foreach (var noise in noiseDatas)
            noise.Initizialize();

        GetComponent<MeshRenderer>().material.mainTexture = texture;
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
        NormalizaMap();
    }

    private void NormalizaMap()
    {
        float maxHeight = float.MinValue;
        float minHeight = float.MaxValue;
        for (int i = 0; i < resolution; i++)
            for (int j = 0; j < resolution; j++)
            {
                if (heightMap[i, j] < minHeight)
                    minHeight = heightMap[i, j];
                else if (heightMap[i, j] > maxHeight)
                    maxHeight = heightMap[i, j];
            }
        for (int i = 0; i < resolution; i++)
            for (int j = 0; j < resolution; j++)
                heightMap[i, j] = Mathf.InverseLerp(minHeight, maxHeight, heightMap[i, j]);

        UpdateColors();
    }

    private void UpdateColors()
    {
        // float stepGrad = 1f / (float)((float)(resolution * 0.5f) - 1f);
        for (int i = 0, n = 0; i < resolution; i++)
        {
            for (int j = 0; j < resolution; j++)
            {
                float value = heightMap[i, j];
                colores[n++] = new Color(value, value, value, 1);
                // float finalValue = Mathf.Clamp01(value - (y * step));
                // float value = fastNoise.GetNoise(x + posX, y + posY) + 1f - (y * stepGrad);
                // float finalValue = value >= 0 ? 1 : 0;
                // colores[n++] = new Color(finalValue, finalValue, finalValue, 1);
            }
        }
        texture.SetPixels(0, 0, resolution, resolution, colores);
        texture.Apply();
    }
}
