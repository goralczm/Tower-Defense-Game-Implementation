using System.Collections.Generic;
using UnityEngine;

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

    [Header("Debug")]
    [SerializeField] private Vector2 _offset;
    [SerializeField] private float _cellSize = .1f;
    [SerializeField] private float _rootCellSize = .3f;
    [SerializeField] private bool _showPath;

    private void OnValidate()
    {
        _startPos.x = Mathf.Clamp(_startPos.x, 0, _width - 1);
        _startPos.y = Mathf.Clamp(_startPos.y, 0, _height - 1);

        _endPos.x = Mathf.Clamp(_endPos.x, 0, _width - 1);
        _endPos.y = Mathf.Clamp(_endPos.y, 0, _height - 1);
    }

    private void OnDrawGizmos()
    {
        MazeGenerator generator = new MazeGenerator(_width, _height, _seed);

        List<Node> maze = generator.GenerateMaze(_steps);

        Vector2Int rootPosition = generator.GetRoot().GetPosition();

        if (_moveRootToEnd)
        {
            generator.MoveRootToPosition(_endPos);
        }

        for (int i = 0; i < maze.Count; i++)
        {
            Vector2 cellPos = new Vector2(maze[i].x, maze[i].y) + _offset;

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(cellPos, _cellSize);

            if (maze[i].next != null)
            {
                Vector2 nextCellPos = new Vector2(maze[i].next.x, maze[i].next.y) + _offset;
                Gizmos.color = Color.green;
                DrawGizmosArrow(cellPos, nextCellPos, Color.green);
            }
        }

        rootPosition = generator.GetRoot().GetPosition();
        Vector2 worldRootPosition = new Vector2(rootPosition.x, rootPosition.y) + _offset;
        Vector2 worldStartPosition = new Vector2(_startPos.x, _startPos.y) + _offset;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(worldRootPosition, _rootCellSize);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(worldStartPosition, _rootCellSize);

        if (_showPath)
        {
            Node curr = generator.GetByCoords(_startPos);

            while (curr.next != null)
            {
                Vector2 currPos = new Vector2(curr.x, curr.y) + _offset;
                Vector2 nextPos = new Vector2(curr.next.x, curr.next.y) + _offset;

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
}
