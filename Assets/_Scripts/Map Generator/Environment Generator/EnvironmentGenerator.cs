using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum TileState
{
    Free,
    Neighbor,
    Occupied
}

public class EnvironmentGenerator : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField, Range(0f, 1f)] private float _randomChance = .2f;

    [Header("References")]
    [SerializeField] private Tilemap _tilemap;

    [Header("Debug")]
    [SerializeField] private bool _debug;

    private Dictionary<Vector2, TileState> _tilesOccupancyMap = new();

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
        UnityEngine.Random.InitState(e.Seed);

        _tilesOccupancyMap.Clear();

        for (float x = e.Bounds.min.x; x <= e.Bounds.max.x; x += _tilemap.cellSize.x)
        {
            for (float y = e.Bounds.min.y; y <= e.Bounds.max.y; y += _tilemap.cellSize.y)
            {
                Vector3Int tileWorldPosition = _tilemap.WorldToCell(new Vector3(x, y, 0));
                TileBase tile = _tilemap.GetTile(tileWorldPosition);

                Vector2 tileCenterPos = _tilemap.GetCellCenterWorld(tileWorldPosition);

                if (tile != null)
                    SetOccupied(tileCenterPos);
                else
                    SetFree(tileCenterPos);
            }
        }
    }

    private void SetOccupied(Vector2 pos)
    {
        _tilesOccupancyMap[pos] = TileState.Occupied;

        foreach (var neighbor in GetNeighbors(pos))
        {
            if (Randomizer.GetRandomBool(1 - _randomChance)) continue;

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
        if (!_debug) return;

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
    }
}
