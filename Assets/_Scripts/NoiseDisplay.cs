using System.Diagnostics.Contracts;
using UnityEngine;

public class NoiseDisplay : MonoBehaviour
{
    [Header("Noise Settings")]
    [SerializeField] private int _width = 100;
    [SerializeField] private int _height = 100;
    [SerializeField] private float _scale = 100f;
    [SerializeField] private int _octaves = 2;
    [SerializeField, Range(0f, 1f)] private float _persistance = 1f;
    [SerializeField] private float _lacunarity = 4f;
    [SerializeField] private int _seed = 42;
    [SerializeField] private Vector2 _offset = Vector2.zero;
    [SerializeField, Range(0f, 2f)] private float _gamma = 1f;
    [SerializeField] private int _contrast = 0;
    [SerializeField] private bool _invert;

    [Header("Noise Pixelization")]
    [SerializeField] private int _pixelSize = 2;

    [Header("Instances")]
    [SerializeField] private SpriteRenderer _rend;

    private void OnValidate()
    {
        if (_width <= 0) _width = 1;
        if (_height <= 0) _height = 1;
        if (_octaves <= 0) _octaves = 1;
        if (_lacunarity < 1) _lacunarity = 1;
        if (_pixelSize < 1) _pixelSize = 1;

        GenerateMap();
    }

    [ContextMenu("Generate Map")]
    private void GenerateMap()
    {
        float[,] noise = NoiseGenerator.GenerateNoise(_width, _height, _seed, _scale, _octaves, _persistance, _lacunarity, _offset);

        Texture2D newTexture = new Texture2D(_width, _height);
        Color[] colorMap = new Color[_width * _height];
        float[,] modifiedNoise = new float[_width, _height];

        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                float noiseValue = ApplyInvertion(ApplyContrast(ApplyGamma(noise[x, y])));

                modifiedNoise[x, y] = noiseValue;
            }
        }

        modifiedNoise = Pixelization.PixelateNoise(modifiedNoise, _width, _height, _pixelSize);

        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                colorMap[y * _width + x] = Color.Lerp(Color.black, Color.white, modifiedNoise[x, y]);
            }
        }

        newTexture.SetPixels(colorMap);
        newTexture.Apply();

        Rect rect = new Rect(0, 0, _width, _height);
        Sprite newSprite = Sprite.Create(newTexture, rect, Vector2.zero, 1);

        _rend.sprite = newSprite;
    }

    private float ApplyGamma(float value)
    {
        return Mathf.Pow(value, 1 / _gamma);
    }

    private float ApplyInvertion(float value)
    {
        if (!_invert)
            return value;

        return 1 - value;
    }

    private float ApplyContrast(float value)
    {
        value *= 255f;

        double factor = (259.0 * (_contrast + 255)) / (255 * (259 - _contrast));

        value = (int)(factor * (value - 128) + 128);

        value = Mathf.Clamp(value, 0, 255);

        return value / 255f;
    }
}
