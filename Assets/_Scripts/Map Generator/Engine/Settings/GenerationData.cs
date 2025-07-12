using UnityEngine;

[System.Serializable]
public class GenerationData
{
    public MazeGenerationSettings _mazeGenerationSettings;
    public int Seed;
    public Vector2Int StartPoint = new(0, 0);
    public Vector2Int EndPoint = new(1000, 1000);

    public void RandomizeStartAndEndPoints()
    {
        StartPoint = _mazeGenerationSettings.GetRandomStartPoint();
        EndPoint = _mazeGenerationSettings.GetRandomEndPoint();

        if (StartPoint == EndPoint)
            RandomizeStartAndEndPoints();
    }

    public bool IsTileOnEdge(Vector2 pos)
    {
        return pos.x == 0 || pos.x == _mazeGenerationSettings.Width - 1 || pos.y == 0 || pos.y == _mazeGenerationSettings.Height - 1;
    }
}
