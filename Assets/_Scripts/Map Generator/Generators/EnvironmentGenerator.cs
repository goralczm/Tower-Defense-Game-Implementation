using MapGenerator.Core;
using MapGenerator.Settings;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utilities;

namespace MapGenerator.Generators
{
    public enum TileState
    {
        Free,
        Neighbor,
        Occupied
    }

    [System.Serializable]
    public class EnvironmentGenerator : IGenerator
    {
        private PathSettings _pathSettings;
        private NoiseSettings _noiseSettings;
        private EnvironmentSettings _environmentSettings;
        private List<GameObject> _obstaclePrefabs;
        private Dictionary<Vector2, TileState> _heatmap = new();
        private List<Vector2> _obstaclePositions = new();
        private List<GameObject> _createdObstacles = new();
        private Tilemap _tilemap;
        private GenerationConfig _generationConfig;
        private Transform _environmentParent;
        private Bounds _bounds;

        public EnvironmentGenerator(EnvironmentSettings environmentSettings, PathSettings pathSettings, GenerationConfig generationData, Tilemap tilemap, List<GameObject> obstaclePrefabs)
        {
            UnityEngine.Random.InitState(generationData.Seed);
            _environmentSettings = environmentSettings;
            _noiseSettings = environmentSettings.NoiseSettings;
            _pathSettings = pathSettings;
            _tilemap = tilemap;
            _generationConfig = generationData;
            _obstaclePrefabs = obstaclePrefabs;
            _bounds = GetPathBounds();
        }

        public Bounds GetPathBounds()
        {
            if (_generationConfig == null || _generationConfig.MazeGenerationSettings == null) return new Bounds();

            return new Bounds(
                new Vector3(_generationConfig.MazeGenerationSettings.Width / 2f,
                    _generationConfig.MazeGenerationSettings.Height / 2f, 0) + _tilemap.transform.position,
                new Vector3(_generationConfig.MazeGenerationSettings.Width + 1,
                    _generationConfig.MazeGenerationSettings.Height + 1, 0));
        }

        public MapLayout Generate(MapLayout layout)
        {
            CreateHeatmap(_environmentSettings.ObstacleNearPathProbability);
            CreateAllObstacles(_environmentSettings.NoiseThreshold);

            return layout;
        }

        private void CreateHeatmap(float obstacleNearPathProbability)
        {
            _heatmap.Clear();

            for (float x = _bounds.min.x; x <= _bounds.max.x; x += _tilemap.cellSize.x)
            {
                for (float y = _bounds.min.y; y <= _bounds.max.y; y += _tilemap.cellSize.y)
                {
                    Vector3Int cellPos = _tilemap.WorldToCell(new Vector3(x, y, 0));
                    Vector2 cellCenterPos = _tilemap.GetCellCenterWorld(cellPos);

                    bool isTileEmpty = _tilemap.GetTile(cellPos) == null;
                    if (isTileEmpty)
                        SetHeatmapCellFree(cellCenterPos);
                    else
                        SetHeatmapCellOccupied(cellCenterPos, obstacleNearPathProbability);
                }
            }
        }

        private void CreateAllObstacles(float noiseThreshold)
        {
            float[,] noise = NoiseGenerator.GenerateNoise(_noiseSettings, _generationConfig.Seed);

            for (int y = 0; y < noise.GetLength(1); y++)
            {
                for (int x = 0; x < noise.GetLength(0); x++)
                {
                    float noiseValue = noise[x, y];
                    if (noiseValue >= noiseThreshold)
                        continue;

                    Vector2 cellCenterPos = _tilemap.GetCellCenterWorld(_tilemap.WorldToCell(new Vector3(_bounds.min.x + x, _bounds.min.y + y, 0)));

                    if (_heatmap.TryGetValue(cellCenterPos, out TileState state) && state == TileState.Free)
                        CreateObstacle(cellCenterPos);
                }
            }
        }

        private void CreateObstacle(Vector2 position)
        {
            if (!_environmentParent)
                _environmentParent = new GameObject("Obstacles").transform;

            GameObject obstacle = UnityEngine.Object.Instantiate(
                _obstaclePrefabs[UnityEngine.Random.Range(0, _obstaclePrefabs.Count)],
                position,
                Quaternion.identity,
                _environmentParent);

            _obstaclePositions.Add(position);
            _createdObstacles.Add(obstacle);
        }

        private void SetHeatmapCellOccupied(Vector2 position, float omitProbability)
        {
            _heatmap[position] = TileState.Occupied;

            foreach (var neighbor in GetNeighbors(position))
            {
                if (Randomizer.GetRandomBool(omitProbability)) continue;

                if (!_heatmap.ContainsKey(neighbor) || _heatmap[neighbor] == TileState.Free)
                    _heatmap[neighbor] = TileState.Neighbor;
            }
        }

        private void SetHeatmapCellFree(Vector2 position)
        {
            _heatmap.TryAdd(position, TileState.Free);
        }

        private List<Vector2> GetNeighbors(Vector2 position)
        {
            List<Vector2> neighbors = new();

            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0)
                        continue;

                    Vector2 neighbor = new Vector2(position.x + dx, position.y + dy);
                    neighbors.Add(neighbor);
                }
            }

            return neighbors;
        }

        public void DrawGizmos(DebugConfig debugConfig)
        {
            if (_pathSettings == null) return;

            float[,] noise;

            switch (debugConfig.EnvironmentDebugView)
            {
                case EnvironmentDebugView.Heatmap:
                    foreach (var occupancy in _heatmap)
                    {
                        switch (occupancy.Value)
                        {
                            case TileState.Free:
                                Gizmos.color = Color.green;
                                break;
                            case TileState.Neighbor:
                                Gizmos.color = Color.yellow;
                                break;
                            case TileState.Occupied:
                                Gizmos.color = Color.red;
                                break;
                        }

                        Gizmos.DrawCube(occupancy.Key, _tilemap.cellSize);
                    }

                    break;
                case EnvironmentDebugView.Noise:
                    noise = NoiseGenerator.GenerateNoise(_noiseSettings, _generationConfig.Seed);

                    for (int y = 0; y < _noiseSettings.Height; y++)
                    {
                        for (int x = 0; x < _noiseSettings.Width; x++)
                        {
                            float noiseValue = noise[x, y];
                            Vector2 cellPos =
                                _tilemap.GetCellCenterWorld(_tilemap.WorldToCell(new Vector3(_bounds.min.x + x, _bounds.min.y + y, 0)));

                            Gizmos.color = Color.Lerp(Color.black, Color.white, noiseValue);
                            Gizmos.DrawCube(cellPos, _tilemap.cellSize);
                        }
                    }

                    break;
                case EnvironmentDebugView.Obstacles:
                    Gizmos.color = Color.red;
                    foreach (var obstaclePos in _obstaclePositions)
                        Gizmos.DrawCube(obstaclePos, _tilemap.cellSize);
                    break;
                case EnvironmentDebugView.None:
                    break;
            }
        }

        public void Cleanup()
        {
            Object.Destroy(_environmentParent.gameObject);
        }
    }
}
