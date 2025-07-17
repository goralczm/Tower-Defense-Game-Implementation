using System;
using System.Collections.Generic;
using _Scripts.Utilities;
using UnityEngine;

[System.Serializable]
public class PathGenerator
{
    /*[Header("Debug")]
    public bool _debug;
    public Vector2 _offset;
    public float _cellSize = .1f;
    public float _rootCellSize = .3f;
    public bool _showPath;
    */

    private PathSettings _pathSettings;
    private GenerationData _generationData;
    private Vector2 _startPos;
    
    public PathGenerator(PathSettings pathSettings, GenerationData generationData, Vector2 startPos)
    {
        _pathSettings = pathSettings;
        _generationData = generationData;
        _startPos = startPos;
    }

    public MazeLayoutGenerator GeneratePath(int depth = 0, bool enforceRules = true)
    {
        if (depth > _pathSettings.MaximumGenerationDepth)
        {
            if (_pathSettings.ExperimentalRandomizeSeedWhenGenerationDepthExceeded)
            {
                _generationData.SetSeed(Randomizer.GetRandomSeed());
                return GeneratePath(0, enforceRules);
            }
            
            Debug.Log("Exceeded generation depth");
            return GenerateBaseMaze();
        }
        
        MazeLayoutGenerator layout = GenerateBaseMaze();
        List<Vector2> waypoints = ExtractWaypoints(layout);

        if (enforceRules)
        {
            if (_pathSettings.EnforceMinimalPathLength)
            {
                float distance = Helpers.CalculatePathLength(waypoints);

                if (distance < _pathSettings.MinimalPathLength)
                {
                    _generationData.SetSeed(_generationData.Seed + 1);
                    return GeneratePath(depth + 1, enforceRules);
                }
            }

            if (_pathSettings.EnforceMaximumStraightTilesInRow)
            {
                float longest = CalculateLongestStraightSection(waypoints);

                if (longest > _pathSettings.MaximumStraightTilesInRow - 1)
                {
                    _generationData.SetSeed(_generationData.Seed + 1);
                    return GeneratePath(depth + 1, enforceRules);
                }
            }
        }

        return layout;
    }
    
    private MazeLayoutGenerator GenerateBaseMaze()
    {
        MazeLayoutGenerator layout = new(_generationData.MazeGenerationSettings.Width, _generationData.MazeGenerationSettings.Height, _generationData.Seed);
        layout.GenerateMaze(_pathSettings.Steps);

        List<Vector2Int> middlePointsToOmit = GetMiddlePointsToOmit();

        foreach (var middle in _generationData.MazeGenerationSettings.MiddlePoints)
        {
            if (middlePointsToOmit.Contains(middle))
                continue;

            layout.MoveRootToPosition(middle);
        }

        if (_pathSettings.MoveRootToEnd)
            layout.MoveRootToPosition(_generationData.GridEndPoint);

        return layout;
    }
    
    private List<Vector2Int> GetMiddlePointsToOmit()
    {
        if (!_generationData.MazeGenerationSettings.RandomlyOmitSomeMiddlePoints)
            return new();

        UnityEngine.Random.InitState(_generationData.Seed);

        int middlePointsCount = _generationData.MazeGenerationSettings.MiddlePoints.Count;
        List<Vector2Int> middlePoints = new List<Vector2Int>(_generationData.MazeGenerationSettings.MiddlePoints);
        middlePoints.Shuffle();
        for (int i = middlePoints.Count - 1; i >= 0; i--)
        {
            if (middlePointsCount > _generationData.MazeGenerationSettings.MinimalMiddlePointsCount && Randomizer.GetRandomBool(_generationData.MazeGenerationSettings.MiddlePointsOmissionProbability))
            {
                middlePointsCount--;
                continue;
            }

            middlePoints.RemoveAt(i);
        }

        return middlePoints;
    }

    private List<Vector2> ExtractWaypoints(MazeLayoutGenerator layoutGenerator)
    {
        var waypoints = new List<Vector2> { ToWorld(_generationData.GridStartPoint) };
        var curr = layoutGenerator.GetByCoords(_generationData.GridStartPoint);

        while (curr?.Next != null)
        {
            Vector2 currDir = curr.Next.GetPosition() - curr.GetPosition();
            Vector2 nextDir = curr.Next.Next != null
                ? curr.Next.Next.GetPosition() - curr.Next.GetPosition()
                : currDir;

            if (currDir != nextDir)
                waypoints.Add(ToWorld(curr.Next.GetPosition()));

            curr = curr.Next;
        }

        waypoints.Add(ToWorld(_generationData.GridEndPoint));
        return waypoints;
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
        return gridPos + _startPos;
    }

    /*private void OnDrawGizmos()
    {
        if (!_debug) return;

        _startPos = transform.position;
        
        MazeLayoutGenerator layoutGenerator = GenerateBaseMaze();
        
        var nodes = layoutGenerator.GetNodes();

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

        List<Vector2Int> middlePointsToOmit = GetMiddlePointsToOmit();

        Gizmos.color = Color.yellow;
        foreach (var middle in _generationData.MazeGenerationSettings.MiddlePoints)
        {
            if (middlePointsToOmit.Contains(middle))
                continue;

            Gizmos.DrawWireSphere(ToWorld(middle), _rootCellSize);
        }

        if (_showPath)
        {
            var curr = layoutGenerator.GetByCoords(_generationData.StartPoint);
            while (curr?.next != null)
            {
                DrawGizmosArrow(ToWorld(curr.GetPosition()), ToWorld(curr.next.GetPosition()), Color.magenta);
                curr = curr.next;
            }
        }
    }*/

    /*public static void DrawGizmosArrow(Vector3 start, Vector3 end, Color color, float headLength = 0.2f, float headAngle = 20f)
    {
        Gizmos.color = color;
        Gizmos.DrawLine(start, end);

        Vector3 direction = (end - start).normalized;
        Vector3 right = Quaternion.Euler(0, 0, headAngle) * -direction;
        Vector3 left = Quaternion.Euler(0, 0, -headAngle) * -direction;

        Gizmos.DrawLine(end, end + right * headLength);
        Gizmos.DrawLine(end, end + left * headLength);
    }*/
}