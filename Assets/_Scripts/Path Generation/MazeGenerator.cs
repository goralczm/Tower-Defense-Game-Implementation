using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public int x;
    public int y;
    public Node next;

    public Node(int x, int y)
    {
        this.x = x;
        this.y = y;
        this.next = null;
    }

    public Vector2Int GetPosition()
    {
        return new Vector2Int(x, y);
    }
}

public class MazeGenerator
{
    private int _width;
    private int _height;
    private int _seed;

    private List<Node> _maze = new();
    private Dictionary<(int, int), Node> _coordedMaze = new();
    private Node _root;
    private System.Random _rnpg;

    public MazeGenerator(int width, int height, int seed)
    {
        _width = width;
        _height = height;
        _seed = seed;
        _rnpg = new System.Random(_seed);
    }

    public List<Node> GenerateMaze(int steps)
    {
        _maze.Clear();
        _coordedMaze.Clear();

        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                Node node = new Node(x, y);

                if (x > 0 && _maze.Count > 0)
                    _maze[_maze.Count - 1].next = node;

                _maze.Add(node);

                if (y > 0 && x == _width - 1)
                    _maze[_width - 1 + (_width * (y - 1))].next = node;

                _coordedMaze.Add((x, y), node);
            }
        }

        
        _root = _maze[_maze.Count - 1];

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

            MoveRoot(offset, true);
        }

        return _maze;
    }

    public void MoveRoot(Vector2Int dir, bool flipOutOfBounds = false)
    {
        Vector2Int rootPosition = _root.GetPosition();
        Vector2Int newRootPosition = rootPosition + dir;

        if (newRootPosition.x > _width - 1)
        {
            if (!flipOutOfBounds)
                return;

            dir = Vector2Int.left;
        }

        if (newRootPosition.x < 0)
        {
            if (!flipOutOfBounds)
                return;

            dir = Vector2Int.right;
        }

        if (newRootPosition.y > _height - 1)
        {
            if (!flipOutOfBounds)
                return;

            dir = Vector2Int.down;
        }

        if (newRootPosition.y < 0)
        {
            if (!flipOutOfBounds)
                return;

            dir = Vector2Int.up;
        }

        newRootPosition = rootPosition + dir;

        _root = _coordedMaze[(newRootPosition.x, newRootPosition.y)];
        _coordedMaze[(rootPosition.x, rootPosition.y)].next = _root;

        _root.next = null;
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
                    MoveRoot(new Vector2Int((int)Mathf.Sign(diff.x), 0));
                    xStepsLeft--;
                }
                else
                {
                    MoveRoot(new Vector2Int(0, (int)Mathf.Sign(diff.y)));
                    yStepsLeft--;
                }
            }
            else
            {
                if (yStepsLeft > 0)
                {
                    MoveRoot(new Vector2Int(0, (int)Mathf.Sign(diff.y)));
                    yStepsLeft--;
                    
                }
                else
                {
                    MoveRoot(new Vector2Int((int)Mathf.Sign(diff.x), 0));
                    xStepsLeft--;
                }
            }
        }
    }

    public Node GetRoot() => _root;

    public Node GetByCoords(Vector2Int coords) => _coordedMaze[(coords.x, coords.y)];
    public List<Node> GetNodes() => _maze;
}
