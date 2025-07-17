using UnityEngine;

[System.Serializable]
public class MapGenerationContext
{
    public PathPreset PathPreset;
    public int Seed = 42;
    public Vector2Int GridStartPoint;
    public Vector2Int GridEndPoint;
    public bool GenerateEnvironment = true;
}
