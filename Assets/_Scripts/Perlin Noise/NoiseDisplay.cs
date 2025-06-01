using UnityEngine;

public class NoiseDisplay : MonoBehaviour
{
    [Header("Noise Settings")]
    [SerializeField] private NoiseSettings _noiseSettings;

    [Header("Instances")]
    [SerializeField] private SpriteRenderer _rend;

    public void GenerateMap()
    {
        float[,] noise = NoiseGenerator.GenerateNoise(_noiseSettings);

        Texture2D newTexture = new Texture2D(_noiseSettings.Width, _noiseSettings.Height);
        Color[] colorMap = new Color[_noiseSettings.Width * _noiseSettings.Height];

        for (int y = 0; y < _noiseSettings.Height; y++)
        {
            for (int x = 0; x < _noiseSettings.Width; x++)
            {
                colorMap[y * _noiseSettings.Width + x] = Color.Lerp(Color.black, Color.white, noise[x, y]);
            }
        }

        newTexture.SetPixels(colorMap);
        newTexture.Apply();

        Rect rect = new Rect(0, 0, _noiseSettings.Width, _noiseSettings.Height);
        Sprite newSprite = Sprite.Create(newTexture, rect, Vector2.zero, 1);

        _rend.sprite = newSprite;
    }

    private float ApplyGamma(float value)
    {
        return Mathf.Pow(value, 1 / _noiseSettings.Gamma);
    }

    private float ApplyInvertion(float value)
    {
        if (!_noiseSettings.Invert)
            return value;

        return 1 - value;
    }

    private float ApplyContrast(float value)
    {
        value *= 255f;

        double factor = (259.0 * (_noiseSettings.Contrast + 255)) / (255 * (259 - _noiseSettings.Contrast));

        value = (int)(factor * (value - 128) + 128);

        value = Mathf.Clamp(value, 0, 255);

        return value / 255f;
    }
}
