using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public enum TileState
{
    Free,
    Neighbor,
    Occupied
}

public enum DebugView
{
    Heatmap,
    Noise,
    Obstacles
}

[System.Serializable]
public class EnvironmentGenerator
{
    [Header("Settings")]
    [SerializeField] private List<GameObject> _obstaclePrefabs;

    [Header("Debug")]
    [SerializeField] private bool _debug;
    [SerializeField] private DebugView _debugView;

    private PathSettings _pathSettings;
    private NoiseSettings _noiseSettings;
    private Dictionary<Vector2, TileState> _heatmap = new();
    private List<Vector2> _obstaclePositions = new();
    private List<GameObject> _createdObstacles = new();
    private Tilemap _tilemap;
    private int _seed = 42;
    private Bounds _bounds;

    public static Func<GameObject, Task> OnObstaclePlacedAsync;
    public static Func<Vector2, Task> OnObstacleDestroyedAsync;

    public void SetTilemap(Tilemap tilemap) => _tilemap = tilemap;
    
    public async Task GenerateEnvironment(PathPreset pathPreset, Bounds bounds, int seed)
    {
        InitializeConfiguration(pathPreset, bounds, seed);
        await DestroyAllObstacles();
        CreateHeatmap(bounds, pathPreset.EnvironmentSettings.ObstacleNearPathProbability);
        await CreateAllObstacles(bounds, pathPreset.EnvironmentSettings.NoiseThreshold);
    }
    
    public void InitializeConfiguration(PathPreset pathPreset, Bounds bounds, int seed)
    {
        _seed = seed;
        _noiseSettings = pathPreset.EnvironmentSettings.NoiseSettings;
        _pathSettings = pathPreset.PathSettings;
        _bounds = bounds;
        Random.InitState(_seed);
    }

    public async Task DestroyAllObstacles()
    {
        _obstaclePositions.Clear();

        for (int i = _createdObstacles.Count - 1; i >= 0; i--)
        {
            Vector2 obstaclePosition = _createdObstacles[i].transform.position;
            
            Object.Destroy(_createdObstacles[i]);
            
            if (OnObstacleDestroyedAsync != null)
                await OnObstacleDestroyedAsync.Invoke(obstaclePosition);
        }
        
        _createdObstacles.Clear();
    }

    private void CreateHeatmap(Bounds bounds, float obstacleNearPathProbability)
    {
        _heatmap.Clear();
        
        for (float x = bounds.min.x; x <= bounds.max.x; x += _tilemap.cellSize.x)
        {
            for (float y = bounds.min.y; y <= bounds.max.y; y += _tilemap.cellSize.y)
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

    private async Task CreateAllObstacles(Bounds bounds, float noiseThreshold)
    {
        float[,] noise = NoiseGenerator.GenerateNoise(_noiseSettings, _seed);

        for (int y = 0; y < noise.GetLength(1); y++)
        {
            for (int x = 0; x < noise.GetLength(0); x++)
            {
                float noiseValue = noise[x, y];
                if (noiseValue >= noiseThreshold)
                    continue;
                
                Vector2 cellCenterPos = _tilemap.GetCellCenterWorld( _tilemap.WorldToCell(new Vector3(bounds.min.x + x, bounds.min.y + y, 0)));

                if (_heatmap.TryGetValue(cellCenterPos, out TileState state) && state == TileState.Free)
                    await CreateObstacle(cellCenterPos);
            }
        }
    }

    private async Task CreateObstacle(Vector2 position)
    {
        _obstaclePositions.Add(position);
        GameObject obstacle = Object.Instantiate(_obstaclePrefabs[Random.Range(0, _obstaclePrefabs.Count)], position,
            Quaternion.identity);
        
        _createdObstacles.Add(obstacle);

        if (OnObstaclePlacedAsync != null)
            await OnObstaclePlacedAsync.Invoke(obstacle);
    }

    private Vector2 GetMaxHeatmapCell()
    {
        return new Vector2(
            _heatmap.Keys.Max(k => k.x),
            _heatmap.Keys.Max(k => k.y)
        );
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

    public void OnDrawGizmos()
    {
        if (!_debug || _pathSettings == null) return;

        float maxX = 0;
        float maxY = 0;
        float[,] noise;

        switch (_debugView)
        {
            case DebugView.Heatmap:
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
            case DebugView.Noise:
                noise = NoiseGenerator.GenerateNoise(_noiseSettings, _seed);

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
            case DebugView.Obstacles:
                Gizmos.color = Color.red;
                foreach (var obstaclePos in _obstaclePositions)
                    Gizmos.DrawCube(obstaclePos, _tilemap.cellSize);
                break;
        }
    }
}