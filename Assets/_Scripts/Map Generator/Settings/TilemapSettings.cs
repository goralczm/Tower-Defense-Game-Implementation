using MapGenerator.Core;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MapGenerator.Settings
{
    [System.Serializable]
    public struct TileConfig
    {
        public Tile Tile;
        public NodeType Type;
    }

    [CreateAssetMenu(menuName = "Path Generation/Tilemap Settings", fileName = "New Tilemap Settings")]
    public class TilemapSettings : ScriptableObject
    {
        [Header("Tiles")]
        public Tile Roundabout;
        public Tile Horizontal;
        public Tile Vertical;
        public Tile LeftTopCorner;
        public Tile RightTopCorner;
        public Tile LeftBottomCorner;
        public Tile RightBottomCorner;

        public List<TileConfig> TileConfigs = new();

        private Dictionary<Vector2, List<TileBase>> _cachedAdjacencyRules = new();
        private Dictionary<TileBase, List<Vector2>> _openSides = new();

        public Tile GetTileByType(NodeType type) => TileConfigs.First(tc => tc.Type == type).Tile;

        public bool CanHaveAdjacency(TileBase tile, Vector2 dir)
        {
            InitializeTiles();

            return _openSides[tile].Contains(dir);
        }

        public List<TileBase> GetPossibleAdjacentTiles(Vector2 dir)
        {
            if (_cachedAdjacencyRules.TryGetValue(dir, out List<TileBase> possibleAdjacentTiles))
                return possibleAdjacentTiles;

            InitializeTiles();

            possibleAdjacentTiles = _openSides.Where(pair => pair.Value.Contains(-dir)).Select(pair => pair.Key).ToList();

            _cachedAdjacencyRules.Add(dir, possibleAdjacentTiles);
            return possibleAdjacentTiles;
        }

        private void InitializeTiles()
        {
            if (_openSides.Count > 0) return;

            _openSides.Add(GetTileByType(NodeType.Roundabout), new()
            {
                Vector2.up, Vector2.right, Vector2.down, Vector2.left
            });

            _openSides.Add(GetTileByType(NodeType.Horizontal), new()
            {
                Vector2.left, Vector2.right
            });

            _openSides.Add(GetTileByType(NodeType.Vertical), new()
            {
                Vector2.down, Vector2.up
            });

            _openSides.Add(GetTileByType(NodeType.LeftTopCorner), new()
            {
                Vector2.left, Vector2.up
            });

            _openSides.Add(GetTileByType(NodeType.RightTopCorner), new()
            {
                Vector2.right, Vector2.up
            });

            _openSides.Add(GetTileByType(NodeType.LeftBottomCorner), new()
            {
                Vector2.left, Vector2.down
            });

            _openSides.Add(GetTileByType(NodeType.RightBottomCorner), new()
            {
                Vector2.right, Vector2.down
            });
        }
    }
}
