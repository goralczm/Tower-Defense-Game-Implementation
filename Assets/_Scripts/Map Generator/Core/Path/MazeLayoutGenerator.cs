using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public int X { get; private set; }
    public int Y { get; private set; }
    public Node Next;

    public Node(int x, int y)
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

public class MazeLayoutGenerator
{
    private int _width;
    private int _height;
    private int _seed;

    private readonly List<Node> _maze = new();
    private readonly Dictionary<(int, int), Node> _mazeByCoords = new();
    private Node _root;
    private System.Random _rnpg;

    public MazeLayoutGenerator(int width, int height, int seed)
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

    public List<Node> GenerateMaze(int steps)
    {
        _maze.Clear();
        _mazeByCoords.Clear();

        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                Node node = new Node(x, y);

                if (x > 0 && _maze.Count > 0)
                    _maze[^1].Next = node;

                _maze.Add(node);

                if (y > 0 && x == _width - 1)
                    _maze[_width - 1 + (_width * (y - 1))].Next = node;

                _mazeByCoords.Add((x, y), node);
            }
        }

        
        _root = _maze[^1];

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

        return _maze;
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

    public Node GetByCoords(Vector2Int coords) => _mazeByCoords[(coords.x, coords.y)];

    public List<Node> GetNodes() => _maze;
}
