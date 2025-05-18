using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathDisplay : MonoBehaviour
{
    [Header("Maze Settings")]
    [SerializeField] private int _width;
    [SerializeField] private int _height;
    [SerializeField] private int _seed;
    [SerializeField] private int _steps;
    [SerializeField] private Vector2Int _startPos;
    [SerializeField] private List<Vector2Int> _middlePoints = new();
    [SerializeField] private Vector2Int _endPos;
    [SerializeField] private bool _moveRootToEnd;
    [SerializeField] private bool _rerollWhenShort;
    [SerializeField] private float _minPathLength;
    [SerializeField] private bool _rerollWhenTooManyStraights;
    [SerializeField] private int _maxStraightTiles;

    [Header("Tilemap Settings")]
    [SerializeField] private RuleTile _pathTile;
    [SerializeField] private Tilemap _tilemap;
    [SerializeField] private Tile _horizontal;
    [SerializeField] private Tile _vertical;
    [SerializeField] private Tile _ltCorner;
    [SerializeField] private Tile _rtCorner;
    [SerializeField] private Tile _lbCorner;
    [SerializeField] private Tile _rbCorner;
    [SerializeField] private Vector2Int _entranceDir;
    [SerializeField] private Vector2Int _exitDir;

    [Header("Debug")]
    [SerializeField] private bool _debug;
    [SerializeField] private Vector2 _offset;
    [SerializeField] private float _cellSize = .1f;
    [SerializeField] private float _rootCellSize = .3f;
    [SerializeField] private bool _showPath;
    [SerializeField] private int _maxGenerationDepth = 30;
    [SerializeField] private bool _experimentalSeedRandomizerOverDepthExceeded;

    public void SetSeed(int seed) { _seed = seed; OnValidate(); }
    public void SetStartPoint(Vector2Int point) { _startPos = point; OnValidate(); }
    public void SetEndPoint(Vector2Int point) { _endPos = point; OnValidate(); }
    public int GetWidth() => _width;
    public int GetHeight() => _height;
    public void SetRandomSeed() { SetSeed(UnityEngine.Random.Range(-100000, 100000)); }
    public void SetEntranceDir(Vector2Int dir) { _entranceDir = dir; }
    public void SetExitDir(Vector2Int dir) { _exitDir = dir; }

    public static Action OnPathGenerated;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Randomize();

        if (Input.GetKeyDown(KeyCode.E))
        {
            _steps--;
            GenerateTilemap();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            _steps++;
            GenerateTilemap();
        }
    }

    private void OnValidate()
    {
        _width = Mathf.Max(2, _width);
        _height = Mathf.Max(2, _height);
        _startPos = ClampToBounds(_startPos);
        _endPos = ClampToBounds(_endPos);
        _minPathLength = Mathf.Min(_minPathLength, 40);
        _maxGenerationDepth = Mathf.Min(_maxGenerationDepth, 200);
    }

    public List<Vector2> GetWaypoints(MazeGenerator generator)
    {
        return ExtractWaypoints(generator);
    }

    public List<Vector2> GetWaypoints()
    {
        MazeGenerator generator = GenerateMazeAndFollowRoute();
        return ExtractWaypoints(generator);
    }

    [ContextMenu("Generate Tilemap")]
    public void GenerateTilemap()
    {
        _tilemap.ClearAllTiles();
        MazeGenerator generator = GenerateMazeAndFollowRoute();
        DrawPath(generator);
        DrawCorners(generator);
        OnPathGenerated?.Invoke();
    }

    private MazeGenerator GenerateMazeAndFollowRoute(int depth = 0)
    {
        MazeGenerator generator = new(_width, _height, _seed);
        generator.GenerateMaze(_steps);

        foreach (var middle in _middlePoints)
            generator.MoveRootToPosition(middle);

        if (_moveRootToEnd)
            generator.MoveRootToPosition(_endPos);

        if (depth < _maxGenerationDepth)
        {
            List<Vector2> waypoints = ExtractWaypoints(generator);

            if (_rerollWhenShort)
            {
                float distance = CalculatePathLength(waypoints);

                if (distance < _minPathLength)
                {
                    _seed++;
                    return GenerateMazeAndFollowRoute(depth + 1);
                }
            }

            if (_rerollWhenTooManyStraights)
            {
                float longest = CalculateLongestStraightSection(waypoints);

                if (longest > _maxStraightTiles)
                {
                    _seed++;
                    return GenerateMazeAndFollowRoute(depth + 1);
                }
            }
        }
        else
        {
            if (_experimentalSeedRandomizerOverDepthExceeded)
            {
                SetRandomSeed();
                return GenerateMazeAndFollowRoute(0);
            }
        }
        

        return generator;
    }

    public float CalculatePathLength(List<Vector2> waypoints)
    {
        float length = 0;

        for (int i = 0; i < waypoints.Count - 1; i++)
            length += Vector2.Distance(waypoints[i], waypoints[i + 1]);

        return length;
    }

    public float CalculateLongestStraightSection(List<Vector2> waypoints)
    {
        float longest = 0;

        for (int i = 0; i < waypoints.Count - 1; i++)
            longest = Mathf.Max(longest, Vector2.Distance(waypoints[i], waypoints[i + 1]));

        return longest;
    }

    private List<Vector2> ExtractWaypoints(MazeGenerator generator)
    {
        var waypoints = new List<Vector2> { ToWorld(_startPos - _entranceDir), ToWorld(_startPos) };
        var curr = generator.GetByCoords(_startPos);

        while (curr?.next != null)
        {
            Vector2 currDir = curr.next.GetPosition() - curr.GetPosition();
            Vector2 nextDir = curr.next.next != null
                ? curr.next.next.GetPosition() - curr.next.GetPosition()
                : currDir;

            if (currDir != nextDir)
                waypoints.Add(ToWorld(curr.next.GetPosition()));

            curr = curr.next;
        }

        waypoints.Add(ToWorld(_endPos));
        waypoints.Add(ToWorld(_endPos + _exitDir));
        return waypoints;
    }

    private Task DrawPath(MazeGenerator generator)
    {
        var curr = generator.GetByCoords(_startPos);
        Vector2Int? prevPos = null;

        while (curr?.next != null)
        {
            Vector2Int pos = curr.GetPosition();
            Vector3Int tilePos = new(pos.x, pos.y, 0);
            Vector2 dir = curr.next.GetPosition() - pos;

            if (!prevPos.HasValue)
            {
                Vector3Int overflowTile = new(pos.x - _entranceDir.x, pos.y - _entranceDir.y, 0);
                if (_entranceDir.x != 0)
                    _tilemap.SetTile(overflowTile, _horizontal);
                else if (_entranceDir.y != 0)
                    _tilemap.SetTile(overflowTile, _vertical);
            }

            if (dir.x != 0)
                _tilemap.SetTile(tilePos, _horizontal);
            else if (dir.y != 0)
                _tilemap.SetTile(tilePos, _vertical);

            prevPos = pos;
            curr = curr.next;
        }

        if (prevPos.HasValue && curr != null)
        {
            Vector3Int lastTile = new(curr.x, curr.y, 0);
            Vector2 lastDir = prevPos.Value - curr.GetPosition();

            if (lastDir.x != 0)
                _tilemap.SetTile(lastTile, _horizontal);
            else if (lastDir.y != 0)
                _tilemap.SetTile(lastTile, _vertical);

            Vector3Int overflowTile = new(curr.x + _exitDir.x, curr.y + _exitDir.y, 0);
            if (_exitDir.x != 0)
                _tilemap.SetTile(overflowTile, _horizontal);
            else if (_exitDir.y != 0)
                _tilemap.SetTile(overflowTile, _vertical);
        }

        return Task.CompletedTask;
    }

    private Task DrawCorners(MazeGenerator generator)
    {
        var curr = generator.GetByCoords(_startPos);

        {
            Vector2Int pos = generator.GetByCoords(_startPos).GetPosition();
            Vector2 dir = curr.next.GetPosition() - pos;

            if (_entranceDir != dir)
            {
                Vector3Int cornerPos = new(_startPos.x, _startPos.y, 0);
                _tilemap.SetTile(cornerPos, GetCornerTile(_entranceDir, dir));
            }
        }

        while (curr?.next?.next != null)
        {
            Vector2Int pos = curr.GetPosition();
            Vector2 dir = curr.next.GetPosition() - pos;
            Vector2 nextDir = curr.next.next.GetPosition() - curr.next.GetPosition();

            if (dir != nextDir)
            {
                Vector3Int cornerPos = new(curr.next.x, curr.next.y, 0);
                _tilemap.SetTile(cornerPos, GetCornerTile(dir, nextDir));
            }

            curr = curr.next;
        }

        if (curr != null)
        {
            Vector2Int pos = new(curr.x, curr.y);
            Vector2 dir = _endPos - pos;
            Vector2 nextDir = (_endPos + _exitDir) - _endPos;

            if (nextDir != dir)
            {
                Vector3Int cornerPos = new(_endPos.x, _endPos.y, 0);
                _tilemap.SetTile(cornerPos, GetCornerTile(dir, nextDir));
            }
        }

        return Task.CompletedTask;
    }

    private Tile GetCornerTile(Vector2 fromDir, Vector2 toDir)
    {
        if (fromDir == Vector2.left && toDir == Vector2.up) return _rtCorner;
        if (fromDir == Vector2.left && toDir == Vector2.down) return _rbCorner;
        if (fromDir == Vector2.right && toDir == Vector2.up) return _ltCorner;
        if (fromDir == Vector2.right && toDir == Vector2.down) return _lbCorner;
        if (fromDir == Vector2.up && toDir == Vector2.left) return _lbCorner;
        if (fromDir == Vector2.up && toDir == Vector2.right) return _rbCorner;
        if (fromDir == Vector2.down && toDir == Vector2.left) return _ltCorner;
        if (fromDir == Vector2.down && toDir == Vector2.right) return _rtCorner;

        return null;
    }

    private Vector2Int ClampToBounds(Vector2Int pos) =>
        new(Mathf.Clamp(pos.x, 0, _width - 1), Mathf.Clamp(pos.y, 0, _height - 1));

    private Vector2 ToWorld(Vector2Int gridPos) => (Vector2)gridPos + (Vector2)transform.position + _offset;

    [ContextMenu("Randomize")]
    public void Randomize()
    {
        Vector2Int startPos = new Vector2Int(0, UnityEngine.Random.Range(0, GetHeight()));
        Vector2Int endPos = new Vector2Int(GetWidth(), UnityEngine.Random.Range(0, GetHeight()));

        Vector2Int entranceDir = Vector2Int.right;
        if (startPos.y == 0)
            entranceDir = Vector2Int.up;
        else if (startPos.y == GetHeight() - 1)
            entranceDir = Vector2Int.down;

        Vector2Int exitDir = Vector2Int.right;
        if (endPos.y == 0)
            exitDir = Vector2Int.down;
        else if (endPos.y == GetHeight() - 1)
            exitDir = Vector2Int.up;

        SetStartPoint(startPos);
        SetEndPoint(endPos);
        SetRandomSeed();
        SetEntranceDir(entranceDir);
        SetExitDir(exitDir);
        GenerateTilemap();
    }

    private void OnDrawGizmos()
    {
        if (!_debug) return;

        MazeGenerator generator = GenerateMazeAndFollowRoute();
        var nodes = generator.GetNodes();

        foreach (var node in nodes)
        {
            Vector2 pos = ToWorld(node.GetPosition());
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(pos, _cellSize);

            if (node.next != null)
                DrawGizmosArrow(pos, ToWorld(node.next.GetPosition()), Color.green);
        }

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(ToWorld(_startPos), _rootCellSize);

        foreach (var middle in _middlePoints)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(ToWorld(middle), _rootCellSize);
        }

        if (_showPath)
        {
            var curr = generator.GetByCoords(_startPos);
            while (curr?.next != null)
            {
                DrawGizmosArrow(ToWorld(curr.GetPosition()), ToWorld(curr.next.GetPosition()), Color.magenta);
                curr = curr.next;
            }
        }
    }

    public static void DrawGizmosArrow(Vector3 start, Vector3 end, Color color, float headLength = 0.2f, float headAngle = 20f)
    {
        Gizmos.color = color;
        Gizmos.DrawLine(start, end);

        Vector3 direction = (end - start).normalized;
        Vector3 right = Quaternion.Euler(0, 0, headAngle) * -direction;
        Vector3 left = Quaternion.Euler(0, 0, -headAngle) * -direction;

        Gizmos.DrawLine(end, end + right * headLength);
        Gizmos.DrawLine(end, end + left * headLength);
    }
}
