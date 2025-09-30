using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MapGenerator.Core
{
    public class MapNode
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public MapNode Next;
        public NodeType Type;

        public MapNode(int x, int y)
        {
            X = x;
            Y = y;
            Next = null;
        }

        public Vector2Int GetPosition()
        {
            return new Vector2Int(X, Y);
        }
    }

    public class MapLayout
    {
        public Vector2Int StartPoint;
        public Vector2Int EndPoint;

        private int _width;
        private int _height;
        private int _seed;

        private readonly Dictionary<(int, int), MapNode> _mazeByCoords = new();
        private MapNode _root;
        private System.Random _rnpg;

        public MapLayout()
        {
            _width = 0;
            _height = 0;
            SetSeed(0);
        }

        public MapLayout(int width, int height, int seed)
        {
            _width = width;
            _height = height;
            SetSeed(seed);
        }

        public void SetSeed(int seed)
        {
            _seed = seed;
            _rnpg = new System.Random(_seed);
        }

        public List<MapNode> GetNodes() => _mazeByCoords.Values.ToList();
        public void GenerateMaze(int steps)
        {
            List<MapNode> maze = new();
            _mazeByCoords.Clear();

            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    MapNode node = new MapNode(x, y);

                    if (x > 0 && maze.Count > 0)
                        maze[^1].Next = node;

                    maze.Add(node);

                    if (y > 0 && x == _width - 1)
                        maze[_width - 1 + (_width * (y - 1))].Next = node;

                    _mazeByCoords.Add((x, y), node);
                }
            }


            _root = maze[^1];

            for (int i = 0; i < steps; i++)
            {
                Vector2Int offset = Vector2Int.zero;
                switch (_rnpg.Next(1, 5))
                {
                    case 1:
                        offset = Vector2Int.up;
                        break;
                    case 2:
                        offset = Vector2Int.right;
                        break;
                    case 3:
                        offset = Vector2Int.down;
                        break;
                    case 4:
                        offset = Vector2Int.left;
                        break;
                }

                MoveRootByDir(offset, true);
            }
        }

        private void MoveRootByDir(Vector2Int dir, bool flipDirWhenOutOfBounds = false)
        {
            Vector2Int rootPosition = _root.GetPosition();
            Vector2Int newRootPosition = rootPosition + dir;

            if (newRootPosition.x > _width - 1)
            {
                if (!flipDirWhenOutOfBounds)
                    return;

                dir = Vector2Int.left;
            }

            if (newRootPosition.x < 0)
            {
                if (!flipDirWhenOutOfBounds)
                    return;

                dir = Vector2Int.right;
            }

            if (newRootPosition.y > _height - 1)
            {
                if (!flipDirWhenOutOfBounds)
                    return;

                dir = Vector2Int.down;
            }

            if (newRootPosition.y < 0)
            {
                if (!flipDirWhenOutOfBounds)
                    return;

                dir = Vector2Int.up;
            }

            newRootPosition = rootPosition + dir;

            _root = _mazeByCoords[(newRootPosition.x, newRootPosition.y)];
            _mazeByCoords[(rootPosition.x, rootPosition.y)].Next = _root;

            _root.Next = null;
        }

        public void MoveRootToPosition(Vector2Int position)
        {
            Vector2Int diff = position - _root.GetPosition();

            int xStepsLeft = Mathf.Abs(diff.x);
            int yStepsLeft = Mathf.Abs(diff.y);

            for (int i = 0; i < Mathf.Abs(diff.x) + Mathf.Abs(diff.y); i++)
            {
                if (_rnpg.Next(0, 2) == 0)
                {
                    if (xStepsLeft > 0)
                    {
                        MoveRootByDir(new Vector2Int((int)Mathf.Sign(diff.x), 0));
                        xStepsLeft--;
                    }
                    else
                    {
                        MoveRootByDir(new Vector2Int(0, (int)Mathf.Sign(diff.y)));
                        yStepsLeft--;
                    }
                }
                else
                {
                    if (yStepsLeft > 0)
                    {
                        MoveRootByDir(new Vector2Int(0, (int)Mathf.Sign(diff.y)));
                        yStepsLeft--;

                    }
                    else
                    {
                        MoveRootByDir(new Vector2Int((int)Mathf.Sign(diff.x), 0));
                        xStepsLeft--;
                    }
                }
            }
        }

        public MapNode GetByCoords(Vector2Int coords)
        {
            if (!_mazeByCoords.TryGetValue((coords.x, coords.y), out MapNode node))
            {
                node = new(coords.x, coords.y);
                _mazeByCoords.Add((coords.x, coords.y), node);
            }

            return node;
        }

        public void SetTile(Vector2Int tilePos, NodeType nodeType)
        {
            GetByCoords(tilePos).Type = nodeType;
        }

        public NodeType GetCornerTile(Vector2 fromDir, Vector2 toDir)
        {
            if (fromDir == Vector2.left && toDir == Vector2.up) return NodeType.RightTopCorner;
            if (fromDir == Vector2.left && toDir == Vector2.down) return NodeType.RightBottomCorner;
            if (fromDir == Vector2.right && toDir == Vector2.up) return NodeType.LeftTopCorner;
            if (fromDir == Vector2.right && toDir == Vector2.down) return NodeType.LeftBottomCorner;
            if (fromDir == Vector2.up && toDir == Vector2.left) return NodeType.LeftBottomCorner;
            if (fromDir == Vector2.up && toDir == Vector2.right) return NodeType.RightBottomCorner;
            if (fromDir == Vector2.down && toDir == Vector2.left) return NodeType.LeftTopCorner;
            if (fromDir == Vector2.down && toDir == Vector2.right) return NodeType.RightTopCorner;

            return NodeType.Empty;
        }
    }

}