using MapGenerator.Core;
using MapGenerator.Settings;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Extensions;
using Utilities;
using UnityEngine.Tilemaps;

namespace MapGenerator.Generators
{
    [System.Serializable]
    public class PathLayoutGenerator : IGenerator
    {
        private PathSettings _pathSettings;
        private GenerationConfig _generationConfig;
        private bool _enforceRules;

        public PathLayoutGenerator(PathSettings pathSettings, GenerationConfig generationData, bool enfroceRules)
        {
            _pathSettings = pathSettings;
            _generationConfig = generationData;
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
                    _generationConfig.SetSeed(MapGenerator.Utilities.Randomizer.GetRandomSeed());
                    return GeneratePath(0, enforceRules);
                }

                Debug.Log("Exceeded generation depth");
                return GenerateBaseMaze(_generationConfig.Seed + depth);
            }

            MapLayout layout = GenerateBaseMaze(_generationConfig.Seed + depth);
            List<Vector2> waypoints = ExtractWaypoints(layout);

            if (enforceRules)
            {
                if (_pathSettings.EnforceMinimalPathLength)
                {
                    float distance = Paths.Utilities.Helpers.CalculatePathLength(waypoints);

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

            layout.StartPoint = _generationConfig.GridStartPoint;
            layout.EndPoint = _generationConfig.GridEndPoint;

            return layout;
        }

        private MapLayout GenerateBaseMaze(int seed)
        {
            MapLayout layout = new(_generationConfig.MazeGenerationSettings.Width, _generationConfig.MazeGenerationSettings.Height, seed);
            layout.GenerateMaze(_pathSettings.Steps);

            List<Vector2Int> middlePointsToOmit = GetMiddlePointsToOmit();

            foreach (var middle in _generationConfig.MazeGenerationSettings.MiddlePoints)
            {
                if (middlePointsToOmit.Contains(middle))
                    continue;

                layout.MoveRootToPosition(middle);
            }

            layout.MoveRootToPosition(_generationConfig.GridEndPoint);

            return layout;
        }

        private List<Vector2Int> GetMiddlePointsToOmit()
        {
            if (!_generationConfig.MazeGenerationSettings.RandomlyOmitSomeMiddlePoints)
                return new();

            UnityEngine.Random.InitState(_generationConfig.Seed);

            int middlePointsCount = _generationConfig.MazeGenerationSettings.MiddlePoints.Count;
            List<Vector2Int> middlePoints = new List<Vector2Int>(_generationConfig.MazeGenerationSettings.MiddlePoints);
            middlePoints.Shuffle();
            for (int i = middlePoints.Count - 1; i >= 0; i--)
            {
                if (middlePointsCount > _generationConfig.MazeGenerationSettings.MinimalMiddlePointsCount && Randomizer.GetRandomBool(_generationConfig.MazeGenerationSettings.MiddlePointsOmissionProbability))
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
            var waypoints = new List<Vector2> { _generationConfig.GridStartPoint };
            var curr = layoutGenerator.GetByCoords(_generationConfig.GridStartPoint);

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

            waypoints.Add(_generationConfig.GridEndPoint);
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

        public void DrawGizmos(DebugConfig debugConfig)
        {
            //noop
        }
    }
}
