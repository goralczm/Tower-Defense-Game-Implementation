using System;
using UnityEngine;

[System.Serializable]
public class GenerationData
{
    public MazeGenerationSettings MazeGenerationSettings;
    public int Seed { get; private set; }
    public Vector2Int GridStartPoint { get; private set; } = new(0, 0);
    public Vector2Int GridEndPoint { get; private set; } = new(1000, 1000);

    public GenerationData(MazeGenerationSettings mazeSettings, int seed)
    {
        MazeGenerationSettings = mazeSettings;
        SetSeed(seed);
    }

    public GenerationData(MazeGenerationSettings mazeSettings, int seed, Vector2Int gridStartPoint,
        Vector2Int gridEndPoint)
    {
        MazeGenerationSettings = mazeSettings;
        SetSeed(seed);
        SetGridStartPoint(gridStartPoint);
        SetGridEndPoint(gridEndPoint);
    }
    
    public void SetSeed(int seed)
    {
        Seed = seed;
    }

    public void SetGridStartPoint(Vector2Int gridStartPoint)
    {
        GridStartPoint = gridStartPoint;
    }
    
    public void SetGridEndPoint(Vector2Int gridEndPoint)
    {
        GridEndPoint = gridEndPoint;
    }

    public void RandomizeAccessPoints()
    {
        SetGridStartPoint(MazeGenerationSettings.GetRandomStartPoint());
        SetGridEndPoint(MazeGenerationSettings.GetRandomEndPoint());

        if (GridStartPoint == GridEndPoint)
            RandomizeAccessPoints();
    }

    public bool IsTileOnEdge(Vector2 position)
    {
        return position.x == 0 || position.x == MazeGenerationSettings.Width - 1 || position.y == 0 ||
               position.y == MazeGenerationSettings.Height - 1;
    }
}