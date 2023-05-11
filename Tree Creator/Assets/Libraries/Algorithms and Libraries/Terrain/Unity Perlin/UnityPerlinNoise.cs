using UnityEngine;

public static class UnityPerlinNoise
{
    public static float GetNoise(Vector2 position, float frequency = 0.25f, float amplitude = 1f)
    {
        float sample = Mathf.PerlinNoise(position.x * frequency, position.y * frequency);
        return sample * amplitude;
    }
}
