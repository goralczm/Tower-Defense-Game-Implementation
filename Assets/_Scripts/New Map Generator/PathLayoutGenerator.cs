using MapGeneration;
using MapGenerator.Utilities;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PathLayoutGenerator : IGenerator
{
    private PathSettings _pathSettings;
    private GenerationConfig _generationData;
    private bool _enforceRules;
    
    public PathLayoutGenerator(PathSettings pathSettings, GenerationConfig generationData, bool enfroceRules)
    {
        _pathSettings = pathSettings;
        _generationData = generationData;
        _enforceRules = enfroceRules;
    }

    public MapLayout Generate(MapLayout layout)
    {
        return GeneratePath(enforceRules: _enforceRules);
    }

    public MapLayout GeneratePath(int depth = 0, bool enforceRules = true)
    {
        if (depth > _pathSettings.MaximumGenerationDepth)
        {
            if (_pathSettings.ExperimentalRandomizeSeedWhenGenerationDepthExceeded)
            {
                _generationData.SetSeed(Randomizer.GetRandomSeed());
                return GeneratePath(0, enforceRules);
            }
            
            Debug.Log("Exceeded generation depth");
            return GenerateBaseMaze(_generationData.Seed + depth);
        }
        
        MapLayout layout = GenerateBaseMaze(_generationData.Seed + depth);
        List<Vector2> waypoints = ExtractWaypoints(layout);

        if (enforceRules)
        {
            if (_pathSettings.EnforceMinimalPathLength)
            {
                float distance = Helpers.CalculatePathLength(waypoints);

                if (distance < _pathSettings.MinimalPathLength)
                    return GeneratePath(depth + 1, enforceRules);
            }

            if (_pathSettings.EnforceMaximumStraightTilesInRow)
            {
                float longest = CalculateLongestStraightSection(waypoints);

                if (longest > _pathSettings.MaximumStraightTilesInRow - 1)
                    return GeneratePath(depth + 1, enforceRules);
            }
        }

        return layout;
    }
    
    private MapLayout GenerateBaseMaze(int seed)
    {
        MapLayout layout = new(_generationData.MazeGenerationSettings.Width, _generationData.MazeGenerationSettings.Height, seed);
        layout.GenerateMaze(_pathSettings.Steps);

        List<Vector2Int> middlePointsToOmit = GetMiddlePointsToOmit();

        foreach (var middle in _generationData.MazeGenerationSettings.MiddlePoints)
        {
            if (middlePointsToOmit.Contains(middle))
                continue;

            layout.MoveRootToPosition(middle);
        }

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

    private List<Vector2> ExtractWaypoints(MapLayout layoutGenerator)
    {
        var waypoints = new List<Vector2> { _generationData.GridStartPoint };
        var curr = layoutGenerator.GetByCoords(_generationData.GridStartPoint);

        while (curr?.Next != null)
        {
            Vector2 currDir = curr.Next.GetPosition() - curr.GetPosition();
            Vector2 nextDir = curr.Next.Next != null
                ? curr.Next.Next.GetPosition() - curr.Next.GetPosition()
                : currDir;

            if (currDir != nextDir)
                waypoints.Add(curr.Next.GetPosition());

            curr = curr.Next;
        }

        waypoints.Add(_generationData.GridEndPoint);
        return waypoints;
    }
    
    public float CalculateLongestStraightSection(List<Vector2> waypoints)
    {
        float longest = 0;

        for (int i = 0; i < waypoints.Count - 1; i++)
            longest = Mathf.Max(longest, Vector2.Distance(waypoints[i], waypoints[i + 1]));

        return longest;
    }

    public void Cleanup()
    {
        //noop
    }
}