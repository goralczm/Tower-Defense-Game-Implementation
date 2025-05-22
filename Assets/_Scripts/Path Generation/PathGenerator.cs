using System.Collections.Generic;
using UnityEngine;

public class PathGenerator : MonoBehaviour
{
    [Header("Path Settings")]
    [SerializeField] private PathSettings _pathSettings;

    [SerializeField] private GenerationData _generationData;

    [Header("Debug")]
    [SerializeField] private bool _debug;
    [SerializeField] private Vector2 _offset;
    [SerializeField] private float _cellSize = .1f;
    [SerializeField] private float _rootCellSize = .3f;
    [SerializeField] private bool _showPath;

    public void SetPathSettings(PathSettings pathSettings) => _pathSettings = pathSettings;
    public void SetGenerationData(GenerationData generationData) => _generationData = generationData;

    public MazeGenerator GeneratePath(int depth = 0)
    {
        MazeGenerator generator = new(_generationData.Width, _generationData.Height, _generationData.Seed);
        generator.GenerateMaze(_pathSettings.Steps);

        foreach (var middle in _generationData.MiddlePoints)
            generator.MoveRootToPosition(middle);

        if (_pathSettings.MoveRootToEnd)
            generator.MoveRootToPosition(_generationData.EndPoint);

        if (depth < _pathSettings.MaximumGenerationDepth)
        {
            List<Vector2> waypoints = ExtractWaypoints(generator);

            if (_pathSettings.EnforceMinimalPathLength)
            {
                float distance = CalculatePathLength(waypoints);

                if (distance < _pathSettings.MinimalPathLength)
                {
                    _generationData.Seed++;
                    return GeneratePath(depth + 1);
                }
            }

            if (_pathSettings.EnforceMaximumStraightTilesInRow)
            {
                float longest = CalculateLongestStraightSection(waypoints);

                if (longest > _pathSettings.MaximumStraightTilesInRow)
                {
                    _generationData.Seed++;
                    return GeneratePath(depth + 1);
                }
            }
        }
        else
        {
            if (_pathSettings.RandomizeSeedWhenGenerationDepthExceeded)
            {
                _generationData.Seed = Random.Range(-100000, 100000);
                return GeneratePath(0);
            }
        }


        return generator;
    }
    
    private List<Vector2> ExtractWaypoints(MazeGenerator generator)
    {
        var waypoints = new List<Vector2> { ToWorld(_generationData.StartPoint) };
        var curr = generator.GetByCoords(_generationData.StartPoint);

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

        waypoints.Add(ToWorld(_generationData.EndPoint));
        return waypoints;
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

    private Vector2 ToWorld(Vector2Int gridPos)
    {
        return (Vector2)gridPos + (Vector2)transform.position + _offset;
    }

    private void OnDrawGizmos()
    {
        if (!_debug) return;

        MazeGenerator generator = new(_generationData.Width, _generationData.Height, _generationData.Seed);
        
        generator.GenerateMaze(_pathSettings.Steps);

        foreach (var middle in _generationData.MiddlePoints)
            generator.MoveRootToPosition(middle);

        if (_pathSettings.MoveRootToEnd)
            generator.MoveRootToPosition(_generationData.EndPoint);
        
        var nodes = generator.GetNodes();

        foreach (var node in nodes)
        {
            Vector2 pos = ToWorld(node.GetPosition());
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(pos, _cellSize);

            if (node.next != null)
                DrawGizmosArrow(pos, ToWorld(node.next.GetPosition()), Color.green);
        }

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(ToWorld(_generationData.StartPoint), _rootCellSize);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(ToWorld(_generationData.EndPoint), _rootCellSize);

        foreach (var middle in _generationData.MiddlePoints)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(ToWorld(middle), _rootCellSize);
        }

        if (_showPath)
        {
            var curr = generator.GetByCoords(_generationData.StartPoint);
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