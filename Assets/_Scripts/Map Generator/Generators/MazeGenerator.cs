using MapGenerator.Core;
using MapGenerator.Settings;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Extensions;
using Utilities;
using System.Threading.Tasks;
using System;
using System.Threading;

namespace MapGenerator.Generators
{
    [System.Serializable]
    public class MazeGenerator : IGenerator
    {
        [Header("Debug")]
        [SerializeField] private bool _debug = true;
        [SerializeField] private float _nodesRadius = .1f;
        [SerializeField] private Color _nodesColor = Color.teal;
        [SerializeField] private Color _pathColor = Color.green;

        private PathSettings _pathSettings;
        private GenerationConfig _generationConfig;
        private bool _enforceRules;
        private CancellationTokenSource _cts;

        public event Action<string> OnStatusChanged;

        public bool ShowDebug => _debug;
        public List<Type> RequiredGenerators => new();

        public MazeGenerator(PathSettings pathSettings, GenerationConfig generationData, bool enfroceRules)
        {
            _pathSettings = pathSettings;
            _generationConfig = generationData;
            _enforceRules = enfroceRules;
        }

        public async Task<MapLayout> GenerateAsync(MapLayout layout, CancellationTokenSource cts)
        {
            _cts = cts;
            OnStatusChanged?.Invoke("Generating path...");
            return await GenerateMaze(enforceRules: _enforceRules);
        }

        public async Task<MapLayout> GenerateMaze(int depth = 0, bool enforceRules = true)
        {
            _cts.Token.ThrowIfCancellationRequested();
            UnityEngine.Random.InitState(_generationConfig.Seed + depth);

            var middlePointsToOmit = GetMiddlePoints();

            if (depth > _pathSettings.MaximumGenerationDepth)
            {
                if (_pathSettings.ExperimentalRandomizeSeedWhenGenerationDepthExceeded)
                {
                    Debug.Log("Exceeded generation depth");
                    _generationConfig.SetSeed(MapGenerator.Utilities.Randomizer.GetRandomSeed());
                    return await GenerateMaze(0, enforceRules);
                }
            }

            MapLayout layout = await Task.Run(() => GenerateRandomMaze(_generationConfig.Seed + depth, middlePointsToOmit));
            List<Vector2> waypoints = await Task.Run(() => ExtractWaypoints(layout));

            if (enforceRules)
            {
                if (_pathSettings.EnforceMinimalPathLength)
                {
                    float distance = Paths.Utilities.Helpers.CalculatePathLength(waypoints);

                    if (distance < _pathSettings.MinimalPathLength)
                        return await GenerateMaze(depth + 1, enforceRules);
                }

                if (_pathSettings.EnforceMaximumStraightTilesInRow)
                {
                    float longest = CalculateLongestStraightSection(waypoints);

                    if (longest > _pathSettings.MaximumStraightTilesInRow - 1)
                        return await GenerateMaze(depth + 1, enforceRules);
                }
            }

            layout.StartPoint = _generationConfig.GridStartPoint;
            layout.EndPoint = _generationConfig.GridEndPoint;

            return layout;
        }

        private MapLayout GenerateRandomMaze(int seed, List<Vector2Int> middlePointsToOmit)
        {
            _cts.Token.ThrowIfCancellationRequested();

            MapLayout layout = new(_generationConfig.MazeGenerationSettings.Width, _generationConfig.MazeGenerationSettings.Height, seed);
            layout.GenerateRandomMaze(_pathSettings.Steps);

            foreach (var middle in middlePointsToOmit)
                layout.MoveRootToPosition(middle);

            layout.MoveRootToPosition(_generationConfig.GridEndPoint);

            return layout;
        }

        private List<Vector2Int> GetMiddlePoints()
        {
            if (!_generationConfig.MazeGenerationSettings.RandomlyOmitSomeMiddlePoints)
                return new();

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
            var nodes = debugConfig.Layout.GetNodes();

            foreach (var node in nodes)
            {
                Vector2 pos = debugConfig.GetPositionOnTilemap(node.GetPosition());
                Gizmos.color = _nodesColor;
                Gizmos.DrawWireSphere(pos, _nodesRadius);

                if (node.Next != null)
                    GizmosHelpers.DrawGizmosArrow(pos, debugConfig.GetPositionOnTilemap(node.Next.GetPosition()), _pathColor);
            }
        }
    }
}
