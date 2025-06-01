using System.Net.NetworkInformation;
using UnityEngine;

public static class NoiseGenerator
{
    public static float[,] GenerateNoise(int width, int height, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset)
    {
        if (scale <= 0)
            scale = .0001f;

        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 10000) + offset.x;
            float offsetY = prng.Next(-100000, 10000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        float[,] noise = new float[width, height];

        float maxNoiseHeight = float.MinValue;
        float minNoiseWidth = float.MaxValue;

        float halfWidth = width / 2f;
        float halfHeight = height / 2f;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x - halfWidth) / scale * frequency + octaveOffsets[i].x;
                    float sampleY = (y - halfHeight) / scale * frequency + octaveOffsets[i].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if (noiseHeight > maxNoiseHeight)
                    maxNoiseHeight = noiseHeight;

                if (noiseHeight < minNoiseWidth)
                    minNoiseWidth = noiseHeight;

                noise[x, y] = noiseHeight;
            }
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                noise[x, y] = Mathf.InverseLerp(minNoiseWidth, maxNoiseHeight, noise[x, y]);
            }
        }

        return noise;
    }

    public static float[,] GenerateNoise(NoiseSettings settings)
    {
        if (settings.Scale <= 0)
            settings.Scale = .0001f;

        System.Random prng = new System.Random(settings.Seed);
        Vector2[] octaveOffsets = new Vector2[settings.Octaves];
        for (int i = 0; i < settings.Octaves; i++)
        {
            float offsetX = prng.Next(-100000, 10000) + settings.Offset.x;
            float offsetY = prng.Next(-100000, 10000) + settings.Offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        float[,] noise = new float[settings.Width, settings.Height];

        float maxNoiseHeight = float.MinValue;
        float minNoiseWidth = float.MaxValue;

        float halfWidth = settings.Width / 2f;
        float halfHeight = settings.Height / 2f;

        for (int y = 0; y < settings.Height; y++)
        {
            for (int x = 0; x < settings.Width; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < settings.Octaves; i++)
                {
                    float sampleX = (x - halfWidth) / settings.Scale * frequency + octaveOffsets[i].x;
                    float sampleY = (y - halfHeight) / settings.Scale * frequency + octaveOffsets[i].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= settings.Persistance;
                    frequency *= settings.Lacunarity;
                }

                if (noiseHeight > maxNoiseHeight)
                    maxNoiseHeight = noiseHeight;

                if (noiseHeight < minNoiseWidth)
                    minNoiseWidth = noiseHeight;

                noise[x, y] = noiseHeight;
            }
        }

        for (int y = 0; y < settings.Height; y++)
        {
            for (int x = 0; x < settings.Width; x++)
            {
                noise[x, y] = Mathf.InverseLerp(minNoiseWidth, maxNoiseHeight, noise[x, y]);
                float noiseValue = ApplyInvertion(ApplyContrast(ApplyGamma(noise[x, y], settings.Gamma), settings.Contrast), settings.Invert);
                noise[x, y] = noiseValue;
            }
        }

        return noise;
    }

    private static float ApplyGamma(float value, float gamma)
    {
        return Mathf.Pow(value, 1 / gamma);
    }

    private static float ApplyInvertion(float value, bool invert)
    {
        if (!invert)
            return value;

        return 1 - value;
    }

    private static float ApplyContrast(float value, float contrast)
    {
        value *= 255f;

        double factor = (259.0 * (contrast + 255)) / (255 * (259 - contrast));

        value = (int)(factor * (value - 128) + 128);

        value = Mathf.Clamp(value, 0, 255);

        return value / 255f;
    }

    public static float[] GenerateNoise1D(int width, float amplitude, float scale, float offset, int seed)
    {
        System.Random prng = new System.Random(seed);

        float offsetX = prng.Next(-100000, 10000) + offset;

        float[] noise = new float[width];

        for (int x = 0; x < width; x++)
        {
            float point = (x + offsetX) * amplitude;

            noise[x] = Mathf.PerlinNoise(point, 0) * scale;
        }

        return noise;
    }
}
