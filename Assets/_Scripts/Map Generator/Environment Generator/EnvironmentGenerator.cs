using System.Collections.Generic;
using System.Linq;
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
    NoiseAndHeatmap
}

public class EnvironmentGenerator : MonoBehaviour
{
    [Header("Noise Settings")]
    [SerializeField] private NoiseSettings _noiseSettings;

    [Header("References")]
    [SerializeField] private Tilemap _tilemap;

    [Header("Debug")]
    [SerializeField] private bool _debug;
    [SerializeField] private DebugView _debugView;

    private Dictionary<Vector2, TileState> _tilesOccupancyMap = new();
    private PathSettings _pathSettings;

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

        _tilesOccupancyMap.Clear();

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
    }

    private void SetOccupied(Vector2 pos, float omitProbability)
    {
        _tilesOccupancyMap[pos] = TileState.Occupied;

        foreach (var neighbor in GetNeighbors(pos))
        {
            if (Randomizer.GetRandomBool(omitProbability)) continue;

            if (!_tilesOccupancyMap.ContainsKey(neighbor))
                _tilesOccupancyMap[neighbor] = TileState.Neighbor;
            else if (_tilesOccupancyMap[neighbor] == TileState.Free)
                _tilesOccupancyMap[neighbor] = TileState.Neighbor;
        }
    }

    private void SetFree(Vector2 pos)
    {
        if (!_tilesOccupancyMap.ContainsKey(pos))
            _tilesOccupancyMap[pos] = TileState.Free;
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
                foreach (var occupancy in _tilesOccupancyMap)
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
                if (_tilesOccupancyMap.Keys.Count > 0)
                {
                    maxX = _tilesOccupancyMap.Keys.Max(k => k.x);
                    maxY = _tilesOccupancyMap.Keys.Max(k => k.y);
                }

                noise = NoiseGenerator.GenerateNoise(_noiseSettings);

                for (int y = 0; y < _noiseSettings.Height; y++)
                {
                    for (int x = 0; x < _noiseSettings.Width; x++)
                    {
                        float noiseValue = noise[x, y];
                        Vector2 cellPos = _tilemap.GetCellCenterWorld(_tilemap.WorldToCell(new Vector3(x - maxX, y - maxY, 0)));

                        if (noiseValue >= _pathSettings.NoiseThreshold)
                            Gizmos.color = Color.red;
                        else
                            Gizmos.color = Color.green;

                        Gizmos.DrawCube(cellPos, _tilemap.cellSize);
                    }
                }
                break;
            case DebugView.NoiseAndHeatmap:
                if (_tilesOccupancyMap.Keys.Count > 0)
                {
                    maxX = _tilesOccupancyMap.Keys.Max(k => k.x);
                    maxY = _tilesOccupancyMap.Keys.Max(k => k.y);
                }

                noise = NoiseGenerator.GenerateNoise(_noiseSettings);

                for (int y = 0; y < _noiseSettings.Height; y++)
                {
                    for (int x = 0; x < _noiseSettings.Width; x++)
                    {
                        float noiseValue = noise[x, y];
                        Vector2 cellPos = _tilemap.GetCellCenterWorld(_tilemap.WorldToCell(new Vector3(x - maxX, y - maxY, 0)));

                        if (_tilesOccupancyMap.ContainsKey(cellPos) && _tilesOccupancyMap[cellPos] == TileState.Free)
                        {
                            if (noiseValue >= _pathSettings.NoiseThreshold)
                            {
                                Gizmos.color = Color.red;
                                Gizmos.DrawCube(cellPos, _tilemap.cellSize);
                            }
                        }
                    }
                }
                break;
        }
    }
}
