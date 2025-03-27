using System.Collections.Generic;
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
    [SerializeField] private Vector2Int _endPos;
    [SerializeField] private bool _moveRootToEnd;

    [Header("Tilemap Settings")]
    [SerializeField] private RuleTile _grassTile;
    [SerializeField] private RuleTile _pathTile;
    [SerializeField] private Tilemap _tilemap;

    [Header("Debug")]
    [SerializeField] private bool _debug;
    [SerializeField] private Vector2 _offset;
    [SerializeField] private float _cellSize = .1f;
    [SerializeField] private float _rootCellSize = .3f;
    [SerializeField] private bool _showPath;

    private void OnValidate()
    {
        if (_width < 2) _width = 2;
        if (_height < 2) _height = 2;

        _startPos.x = Mathf.Clamp(_startPos.x, 0, _width - 1);
        _startPos.y = Mathf.Clamp(_startPos.y, 0, _height - 1);

        _endPos.x = Mathf.Clamp(_endPos.x, 0, _width - 1);
        _endPos.y = Mathf.Clamp(_endPos.y, 0, _height - 1);

        GenerateTilemap();
    }

    [ContextMenu("Generate Tilemap")]
    private void GenerateTilemap()
    {
        MazeGenerator generator = new MazeGenerator(_width, _height, _seed);

        generator.GenerateMaze(_steps);

        if (_moveRootToEnd)
            generator.MoveRootToPosition(_endPos);

        List<Node> maze = generator.GetNodes();

        _tilemap.ClearAllTiles();

        foreach (var node in maze)
        {
            Vector2Int nodePosition = node.GetPosition();
            Vector3Int nodeWorldPosition = new Vector3Int(nodePosition.x, nodePosition.y, 0);
            _tilemap.SetTile(nodeWorldPosition, _grassTile);
        }

        Node curr = generator.GetByCoords(_startPos);
        while (curr.next != null)
        {
            Vector2Int nodePosition = curr.GetPosition();
            Vector3Int nodeWorldPosition = new Vector3Int(nodePosition.x, nodePosition.y, 0);
            _tilemap.SetTile(nodeWorldPosition, _pathTile);
            curr = curr.next;
        }
        _tilemap.SetTile(new Vector3Int(curr.x, curr.y, 0), _pathTile);
    }

    private void OnDrawGizmos()
    {
        if (!_debug)
            return;
        
        MazeGenerator generator = new MazeGenerator(_width, _height, _seed);

        List<Node> maze = generator.GenerateMaze(_steps);

        Vector2Int rootPosition = generator.GetRoot().GetPosition();

        if (_moveRootToEnd)
        {
            generator.MoveRootToPosition(_endPos);
        }

        for (int i = 0; i < maze.Count; i++)
        {
            Vector2 cellPos = new Vector2(maze[i].x, maze[i].y) + (Vector2)transform.position + _offset;

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(cellPos, _cellSize);

            if (maze[i].next != null)
            {
                Vector2 nextCellPos = new Vector2(maze[i].next.x, maze[i].next.y) + (Vector2)transform.position + _offset;
                Gizmos.color = Color.green;
                DrawGizmosArrow(cellPos, nextCellPos, Color.green);
            }
        }

        rootPosition = generator.GetRoot().GetPosition();
        Vector2 worldRootPosition = new Vector2(rootPosition.x, rootPosition.y) + (Vector2)transform.position + _offset;
        Vector2 worldStartPosition = new Vector2(_startPos.x, _startPos.y) + (Vector2)transform.position + _offset;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(worldRootPosition, _rootCellSize);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(worldStartPosition, _rootCellSize);

        if (_showPath)
        {
            Node curr = generator.GetByCoords(_startPos);

            while (curr.next != null)
            {
                Vector2 currPos = new Vector2(curr.x, curr.y) + (Vector2)transform.position + _offset;
                Vector2 nextPos = new Vector2(curr.next.x, curr.next.y) + (Vector2)transform.position + _offset;

                DrawGizmosArrow(currPos, nextPos, Color.magenta);
                curr = curr.next;
            }
        }
    }

    public static void DrawGizmosArrow(Vector3 start, Vector3 end, Color color, float arrowHeadLength = 0.2f, float arrowHeadAngle = 20.0f)
    {
        Gizmos.color = color;

        Gizmos.DrawLine(start, end);

        Vector3 direction = (end - start).normalized;

        Vector3 rightDirection = Quaternion.Euler(0, 0, arrowHeadAngle) * -direction;
        Vector3 leftDirection = Quaternion.Euler(0, 0, -arrowHeadAngle) * -direction;

        Vector3 rightArrowHead = end + rightDirection * arrowHeadLength;
        Vector3 leftArrowHead = end + leftDirection * arrowHeadLength;

        Gizmos.DrawLine(end, rightArrowHead);
        Gizmos.DrawLine(end, leftArrowHead);
    }

    public void Randomize()
    {
        _seed = Random.Range(-100000, 100000);

        _startPos = new Vector2Int(0, Random.Range(0, _height));
        _endPos = new Vector2Int(_width, Random.Range(0, _height));

        OnValidate();
    }
}
