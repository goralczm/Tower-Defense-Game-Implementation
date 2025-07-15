using System;
using UnityEngine;

[System.Serializable]
public class GenerationData
{
    [HideInInspector] public MazeGenerationSettings MazeGenerationSettings;
    [HideInInspector]public int Seed;
    public Vector2Int StartPoint = new(0, 0);
    public Vector2Int EndPoint = new(1000, 1000);

    public Action<int> OnSeedChanged;

    public void SetSeed(int seed)
    {
        Seed = seed;
        OnSeedChanged?.Invoke(seed);
    }

    public void RandomizeAccessPoints()
    {
        StartPoint = MazeGenerationSettings.GetRandomStartPoint();
        EndPoint = MazeGenerationSettings.GetRandomEndPoint();

        if (StartPoint == EndPoint)
            RandomizeAccessPoints();
    }

    public bool IsTileOnEdge(Vector2 pos)
    {
        return pos.x == 0 || pos.x == MazeGenerationSettings.Width - 1 || pos.y == 0 ||
               pos.y == MazeGenerationSettings.Height - 1;
    }
}