using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

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

public class EnvironmentGenerator : MonoBehaviour
{
    [Header("Noise Settings")]
    [SerializeField] private NoiseSettings _noiseSettings;
    [SerializeField] private List<GameObject> _obstaclePrefabs;

    [Header("References")]
    [SerializeField] private Tilemap _tilemap;

    [Header("Debug")]
    [SerializeField] private bool _debug;
    [SerializeField] private DebugView _debugView;

    private Dictionary<Vector2, TileState> _heatmap = new();
    private List<Vector2> _obstaclePositions = new();
    private PathSettings _pathSettings;
    private List<GameObject> _createdObstacles = new();

    private void OnEnable()
    {
        PathGenerationDirector.OnPathGenerationEnded += GenerateEnvironment;
    }

    private void OnDisable()
    {
        PathGenerationDirector.OnPathGenerationEnded -= GenerateEnvironment;
    }

    private void GenerateEnvironment(object sender, PathGenerationDirector.OnPathGeneratedEventArgs e)
    {
        UnityEngine.Random.InitState(e.GenerationData.Seed);
        _noiseSettings.Seed = e.GenerationData.Seed;
        _pathSettings = e.PathPreset.PathSettings;

        _heatmap.Clear();
        _obstaclePositions.Clear();

        for (int i = _createdObstacles.Count - 1; i >= 0; i--)
        {
            Destroy(_createdObstacles[i]);
        }

        for (float x = e.Bounds.min.x; x <= e.Bounds.max.x; x += _tilemap.cellSize.x)
        {
            for (float y = e.Bounds.min.y; y <= e.Bounds.max.y; y += _tilemap.cellSize.y)
            {
                Vector3Int tileWorldPosition = _tilemap.WorldToCell(new Vector3(x, y, 0));
                TileBase tile = _tilemap.GetTile(tileWorldPosition);

                Vector2 tileCenterPos = _tilemap.GetCellCenterWorld(tileWorldPosition);

                if (tile != null)
                    SetOccupied(tileCenterPos, e.PathPreset.PathSettings.OmitNeighborProbability);
                else
                    SetFree(tileCenterPos);
            }
        }

        float maxX = _heatmap.Keys.Max(k => k.x);
        float maxY = _heatmap.Keys.Max(k => k.y);

        float[,] noise = NoiseGenerator.GenerateNoise(_noiseSettings);

        for (int y = 0; y < _noiseSettings.Height; y++)
        {
            for (int x = 0; x < _noiseSettings.Width; x++)
            {
                float noiseValue = noise[x, y];
                Vector2 cellPos = _tilemap.GetCellCenterWorld(_tilemap.WorldToCell(new Vector3(x - maxX, y - maxY, 0)));

                if (_heatmap.ContainsKey(cellPos) && _heatmap[cellPos] == TileState.Free)
                {
                    if (noiseValue < _pathSettings.NoiseThreshold)
                        _obstaclePositions.Add(cellPos);
                }
            }
        }

        foreach (var obstaclePos in _obstaclePositions)
        {
            _createdObstacles.Add(Instantiate(_obstaclePrefabs.GetRandom(1)[0], obstaclePos, Quaternion.identity));
        }
    }

    private void SetOccupied(Vector2 pos, float omitProbability)
    {
        _heatmap[pos] = TileState.Occupied;

        foreach (var neighbor in GetNeighbors(pos))
        {
            if (Randomizer.GetRandomBool(omitProbability)) continue;

            if (!_heatmap.ContainsKey(neighbor))
                _heatmap[neighbor] = TileState.Neighbor;
            else if (_heatmap[neighbor] == TileState.Free)
                _heatmap[neighbor] = TileState.Neighbor;
        }
    }

    private void SetFree(Vector2 pos)
    {
        if (!_heatmap.ContainsKey(pos))
            _heatmap[pos] = TileState.Free;
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

    private void OnDrawGizmos()
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
                if (_heatmap.Keys.Count > 0)
                {
                    maxX = _heatmap.Keys.Max(k => k.x);
                    maxY = _heatmap.Keys.Max(k => k.y);
                }

                noise = NoiseGenerator.GenerateNoise(_noiseSettings);

                for (int y = 0; y < _noiseSettings.Height; y++)
                {
                    for (int x = 0; x < _noiseSettings.Width; x++)
                    {
                        float noiseValue = noise[x, y];
                        Vector2 cellPos = _tilemap.GetCellCenterWorld(_tilemap.WorldToCell(new Vector3(x - maxX, y - maxY, 0)));

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
