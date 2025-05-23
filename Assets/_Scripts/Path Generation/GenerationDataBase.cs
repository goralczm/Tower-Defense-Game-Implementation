using System.Collections.Generic;
using System.Net;
using UnityEngine;

[CreateAssetMenu(menuName = "Path Generation/Generation Data Base", fileName = "New Generation Data Base")]
public class GenerationDataBase : ScriptableObject
{
    public int Width = 5;
    public int Height = 5;
    public Vector2Int MinStartPoint = new(0, -5);
    public Vector2Int MaxStartPoint = new(0, 5);
    public Vector2Int MinEndPoint = new(5, -5);
    public Vector2Int MaxEndPoint = new(5, 5);
    public List<Vector2Int> MiddlePoints = new();

    public Vector2Int GetRandomStartPoint() => new(Random.Range(MinStartPoint.x, MaxStartPoint.x), Random.Range(MinStartPoint.y, MaxStartPoint.y));

    public Vector2Int GetRandomEndPoint() => new(Random.Range(MinEndPoint.x, MaxEndPoint.x), Random.Range(MinEndPoint.y, MaxEndPoint.y));

    public void OnValidate()
    {
        Width = Mathf.Max(2, Width);
        Height = Mathf.Max(2, Height);
        MinStartPoint = ClampToBounds(MinStartPoint);
        MaxStartPoint = ClampToBounds(MaxStartPoint);
        MinEndPoint = ClampToBounds(MinEndPoint);
        MaxEndPoint = ClampToBounds(MaxEndPoint);
    }

    private Vector2Int ClampToBounds(Vector2Int pos)
    {
        return new(Mathf.Clamp(pos.x, 0, Width - 1), Mathf.Clamp(pos.y, 0, Height - 1));
    }
}
