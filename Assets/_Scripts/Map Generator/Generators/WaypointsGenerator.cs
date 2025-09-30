using MapGenerator.Core;
using MapGenerator.Settings;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MapGenerator.Generators
{
    [System.Serializable]
    public class WaypointsGenerator : IGenerator
    {
        private TilemapSettings _tilemapSettings;
        private Tilemap _tilemap;
        private List<Vector2> _waypoints = new();

        public WaypointsGenerator(TilemapSettings tilemapSettings, Tilemap pathTilemap)
        {
            _tilemapSettings = tilemapSettings;
            _tilemap = pathTilemap;
        }

        public MapLayout Generate(MapLayout layout)
        {
            Vector3Int cellPos = new(layout.StartPoint.x, layout.StartPoint.y, 0);
            Vector2 startPoint = _tilemap.GetCellCenterWorld(cellPos);

            List<Vector2> waypoints = ExtractWaypoints(startPoint);
            Object.FindFirstObjectByType<WaypointsParent>().CacheWaypoints(waypoints);

            return layout;
        }

        public void ClearWaypoints()
        {
            _waypoints.Clear();
        }

        public List<Vector2> ExtractWaypoints(Vector2 startPoint)
        {
            ClearWaypoints();

            Vector2 dir = FindNextDir(startPoint, null);
            Vector2 end = ExtractPointsInDir(startPoint, dir);

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

            return _waypoints;
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
            TileBase startTile = _tilemap.GetTile(_tilemap.WorldToCell(start));
            TileBase nextTile = _tilemap.GetTile(_tilemap.WorldToCell(start + newDir));

            if (lastDir == -newDir)
                return false;

            if (nextTile == null)
                return false;

            if (!_tilemapSettings.CanHaveAdjacency(startTile, newDir))
                return false;

            if (!_tilemapSettings.GetPossibleAdjacentTiles(newDir).Contains(nextTile))
                return false;

            return true;
        }

        private Vector2 ExtractPointsInDir(Vector2 current, Vector2 dir)
        {
            AddWaypoint(current);
            current += dir;

            while (true)
            {
                TileBase tile = _tilemap.GetTile(_tilemap.WorldToCell(current));

                if (tile != null)
                {
                    AddWaypoint(current);

                    if (tile.name.Contains("Corner"))
                        return current;

                    if (!tile.name.ToLower().Contains("roundabout") && !IsDirectionValid(current, -dir, dir))
                        return current;

                    current += dir;
                }
                else
                    return current - dir;
            }
        }

        private void AddWaypoint(Vector2 waypoint)
        {
            if (_waypoints.Contains(waypoint) && !_tilemap.GetTile(_tilemap.WorldToCell(waypoint)).name.ToLower().Contains("roundabout"))
                return;

            _waypoints.Add(waypoint);
        }

        public void Cleanup()
        {
            //noop
        }
    }
}
