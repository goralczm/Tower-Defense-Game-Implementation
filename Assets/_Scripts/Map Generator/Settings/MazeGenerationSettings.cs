using System.Collections.Generic;
using UnityEngine;

namespace MapGenerator.Settings
{
    public enum Side
    {
        Left,
        Right,
        Top,
        Bottom,
        Custom,
    }

    [System.Serializable]
    [CreateAssetMenu(menuName = "Path Generation/Maze Generation Settings", fileName = "New Maze Generation Settings")]
    public class MazeGenerationSettings : ScriptableObject
    {
        public int Width = 5;
        public int Height = 5;
        public List<Vector2Int> MiddlePoints = new();
        public bool RandomlyOmitSomeMiddlePoints = false;
        public float MiddlePointsOmissionProbability = 0.2f;
        public int MinimalMiddlePointsCount = 0;
        public Side StartSide;
        public Side EndSide;

        [HideInInspector] public Vector2Int MinStartPoint = new(0, -5);
        [HideInInspector] public Vector2Int MaxStartPoint = new(0, 5);
        [HideInInspector] public Vector2Int MinEndPoint = new(5, -5);
        [HideInInspector] public Vector2Int MaxEndPoint = new(5, 5);

        public Vector2Int GetMinStartPoint()
        {
            switch (StartSide)
            {
                case Side.Left:
                    return new Vector2Int(0, 0);
                case Side.Right:
                    return new Vector2Int(Width - 1, 0);
                case Side.Top:
                    return new Vector2Int(0, Height - 1);
                case Side.Bottom:
                    return new Vector2Int(0, 0);
                default:
                    return MinStartPoint;
            }
        }

        public Vector2Int GetMaxStartPoint()
        {
            switch (StartSide)
            {
                case Side.Left:
                    return new Vector2Int(0, Height - 1);
                case Side.Right:
                    return new Vector2Int(Width - 1, Height - 1);
                case Side.Top:
                    return new Vector2Int(Width - 1, Height - 1);
                case Side.Bottom:
                    return new Vector2Int(Width - 1, 0);
                default:
                    return MaxStartPoint;
            }
        }

        public Vector2Int GetMinEndPoint()
        {
            switch (EndSide)
            {
                case Side.Right:
                    return new Vector2Int(Width - 1, 0);
                case Side.Left:
                    return new Vector2Int(0, 0);
                case Side.Bottom:
                    return new Vector2Int(0, 0);
                case Side.Top:
                    return new Vector2Int(0, Height - 1);
                default:
                    return MinEndPoint;
            }
        }

        public Vector2Int GetMaxEndPoint()
        {
            switch (EndSide)
            {
                case Side.Right:
                    return new Vector2Int(Width - 1, Height - 1);
                case Side.Left:
                    return new Vector2Int(0, Height - 1);
                case Side.Bottom:
                    return new Vector2Int(Width - 1, 0);
                case Side.Top:
                    return new Vector2Int(Width - 1, Height - 1);
                default:
                    return MaxEndPoint;
            }
        }

        public Vector2Int GetRandomStartPoint()
        {
            Vector2Int minStart = GetMinStartPoint();
            Vector2Int maxStart = GetMaxStartPoint();

            Vector2Int randomStart = new(Random.Range(minStart.x, maxStart.x), Random.Range(minStart.y, maxStart.y));

            return ClampToBounds(randomStart);
        }

        public Vector2Int GetRandomEndPoint()
        {
            Vector2Int minEnd = GetMinEndPoint();
            Vector2Int maxEnd = GetMaxEndPoint();

            Vector2Int randomEnd = new(Random.Range(minEnd.x, maxEnd.x), Random.Range(minEnd.y, maxEnd.y));

            return ClampToBounds(randomEnd);
        }

        public void OnValidate()
        {
            Width = Mathf.Max(2, Width);
            Height = Mathf.Max(2, Height);
        }

        private Vector2Int ClampToBounds(Vector2Int pos)
        {
            return new(Mathf.Clamp(pos.x, 0, Width - 1), Mathf.Clamp(pos.y, 0, Height - 1));
        }
    }
}
