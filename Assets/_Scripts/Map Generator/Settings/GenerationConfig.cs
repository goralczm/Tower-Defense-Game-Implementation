using System;
using UnityEngine;

namespace MapGenerator.Settings
{
    [System.Serializable]
    public class GenerationConfig
    {
        [HideInInspector] [NonSerialized] public MazeGenerationSettings MazeGenerationSettings;
        public int Seed;
        public Vector2Int GridStartPoint;
        public Vector2Int GridEndPoint;
        public bool EnforceRules;
        public bool RenderOverflowTiles;

        public GenerationConfig(MazeGenerationSettings mazeSettings, int seed)
        {
            MazeGenerationSettings = mazeSettings;
            SetSeed(seed);
        }

        public GenerationConfig(MazeGenerationSettings mazeSettings, int seed, Vector2Int gridStartPoint,
            Vector2Int gridEndPoint)
        {
            MazeGenerationSettings = mazeSettings;
            SetSeed(seed);
            SetGridStartPoint(gridStartPoint);
            SetGridEndPoint(gridEndPoint);
        }

        public void OnValidate()
        {
            GridStartPoint = new Vector2Int(
                Mathf.Clamp(GridStartPoint.x, 0, MazeGenerationSettings.Width - 1),
                Mathf.Clamp(GridStartPoint.y, 0, MazeGenerationSettings.Height - 1));

            GridEndPoint = new Vector2Int(
                Mathf.Clamp(GridEndPoint.x, 0, MazeGenerationSettings.Width - 1),
                Mathf.Clamp(GridEndPoint.y, 0, MazeGenerationSettings.Height - 1));
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

        public Vector2Int GetAccessPointDir(Vector2Int accessPoint)
        {
            Vector2Int accessDir = accessPoint.x == 0 ? Vector2Int.right : Vector2Int.left;

            if (accessPoint.y == 0)
                accessDir = Vector2Int.up;
            else if (accessPoint.y == MazeGenerationSettings.Height - 1)
                accessDir = Vector2Int.down;

            return accessDir;
        }
    }
}
