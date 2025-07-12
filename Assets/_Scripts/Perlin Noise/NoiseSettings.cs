using UnityEngine;

[CreateAssetMenu(menuName = "Perlin Noise/Noise Settings", fileName = "New Noise Settings")]
public class NoiseSettings : ScriptableObject
{
    [Header("Noise Settings")]
    public int Width = 100;
    public int Height = 100;
    public float Scale = 100f;
    public int Octaves = 2;
    [Range(0f, 1f)] public float Persistance = 1f;
    public float Lacunarity = 4f;
    public Vector2 Offset = Vector2.zero;
    
    private int _seed = 42;

    [Header("Filters Settings")]
    [Range(0f, 2f)] public float Gamma = 1f;
    public int Contrast = 0;
    public bool Invert;

    public int GetSeed()
    {
        return _seed;
    }
    
    public void SetSeed(int seed)
    {
        _seed = seed;
    }
    
    private void OnValidate()
    {
        if (Width <= 0) Width = 1;
        if (Height <= 0) Height = 1;
        if (Octaves <= 0) Octaves = 1;
        if (Lacunarity < 1) Lacunarity = 1;
    }
}
