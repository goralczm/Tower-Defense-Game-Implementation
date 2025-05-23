using UnityEngine;

[System.Serializable]
public class GenerationData
{
    public GenerationDataBase GenerationDataBase;
    public int Seed;
    public Vector2Int StartPoint = new(0, 0);
    public Vector2Int EndPoint = new(1000, 1000);

    public void RandomizeStartAndEndPoints()
    {
        StartPoint = GenerationDataBase.GetRandomStartPoint();
        EndPoint = GenerationDataBase.GetRandomEndPoint();
    }

    public bool IsTileOnEdge(Vector2 pos)
    {
        return pos.x == 0 || pos.x == GenerationDataBase.Width - 1 || pos.y == 0 || pos.y == GenerationDataBase.Height - 1;
    }
}
