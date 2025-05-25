using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public struct TileAdjecency
{
    public TileBase Tile;
    public Vector2 Direction;
    public List<TileBase> PossibleAdjecentTiles;
}

public class WaypointsExtractor : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Vector2 _startingPoint;
    [SerializeField] private Tilemap _pathTilemap;

    [Header("Adjecency Settings")]
    [SerializeField] private TileAdjecency[] _rules;

    [Header("Debug")]
    [SerializeField] private Color _waypointColor = Color.red;
    [SerializeField] private float _waypointRadius = .23f;

    private List<Vector2> _waypoints = new();
    private Dictionary<(TileBase, Vector2), List<TileBase>> _cachedRules = new();

    public List<Vector2> GetWaypoints() => _waypoints;
    public Vector2 SetStartPoint(Vector2 startPoint) => _startingPoint = startPoint;

    private void Start()
    {
        CacheRules();
    }

    [ContextMenu("Cache Rules")]
    public void CacheRules()
    {
        _cachedRules.Clear();
        foreach (var rule in _rules)
        {
            if (_cachedRules.ContainsKey((rule.Tile, rule.Direction)))
                continue;

            _cachedRules.Add((rule.Tile, rule.Direction), rule.PossibleAdjecentTiles);
        }
    }

    [ContextMenu("Clear Waypoints")]
    public void ClearWaypoints()
    {
        _waypoints.Clear();
    }

    [ContextMenu("Extract Waypoints")]
    public void ExtractWaypoints()
    {
        _waypoints.Clear();

        Vector2 dir = FindNextDir(_startingPoint, null);
        Vector2 end = ExtractPointsInDir(_startingPoint, dir);

        while (true)
        {
            try
            {
                dir = FindNextDir(end, dir);
            }
            catch
            {
                break;
            }

            Vector2 newEnd = ExtractPointsInDir(end, dir);

            if (newEnd == end)
                break;

            end = newEnd;
        }
    }

    private Vector2 FindNextDir(Vector2 start, Vector2? lastDir = null)
    {
        if (IsDirectionValid(start, lastDir, Vector2.right))
            return Vector2.right;
        else if (IsDirectionValid(start, lastDir, Vector2.up))
            return Vector2.up;
        else if (IsDirectionValid(start, lastDir, Vector2.left))
            return Vector2.left;
        else if (IsDirectionValid(start, lastDir, Vector2.down))
            return Vector2.down;

        throw new System.Exception("No valid direction found from the starting point.");
    }

    private bool IsDirectionValid(Vector2 start, Vector2? lastDir, Vector2 newDir)
    {
        TileBase startTile = _pathTilemap.GetTile(_pathTilemap.WorldToCell(start));
        TileBase nextTile = _pathTilemap.GetTile(_pathTilemap.WorldToCell(start + newDir));

        return lastDir != -newDir && nextTile != null && _cachedRules.ContainsKey((startTile, newDir)) && _cachedRules[(startTile, newDir)].Contains(nextTile);
    }

    private Vector2 ExtractPointsInDir(Vector2 current, Vector2 dir)
    {
        AddWaypoint(current);
        current += dir;

        while (true)
        {
            TileBase tile = _pathTilemap.GetTile(_pathTilemap.WorldToCell(current));

            if (tile != null)
            {
                AddWaypoint(current);

                if (tile.name.Contains("Corner"))
                    return current;

                if (!IsDirectionValid(current, -dir, dir))
                    return current;

                current += dir;
            }
            else
                return current - dir;
        }
    }

    private void AddWaypoint(Vector2 waypoint)
    {
        if (_waypoints.Contains(waypoint) && !_pathTilemap.GetTile(_pathTilemap.WorldToCell(waypoint)).name.Contains("Roundabout"))
            return;

        _waypoints.Add(waypoint);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(_startingPoint, _waypointRadius);

        Gizmos.color = _waypointColor;

        foreach (var waypoint in _waypoints)
            Gizmos.DrawWireSphere(waypoint, _waypointRadius);
    }
}
