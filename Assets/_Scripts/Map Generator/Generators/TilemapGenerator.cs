using MapGenerator.Core;
using MapGenerator.Settings;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MapGenerator.Generators
{
    public class TilemapGenerator : IGenerator
    {
        private Tilemap _tilemap;
        private TilemapSettings _tilemapSettings;

        public TilemapGenerator(Tilemap tilemap, TilemapSettings tilemapSettings)
        {
            _tilemap = tilemap;
            _tilemapSettings = tilemapSettings;
        }

        public MapLayout Generate(MapLayout layout)
        {
            _tilemap.ClearAllTiles();

            foreach (var node in layout.GetNodes())
            {
                Tile tile = _tilemapSettings.GetTileByType(node.Type);
                if (tile == null) continue;

                Vector3Int cellPosition = new(node.GetPosition().x, node.GetPosition().y, 0);
                _tilemap.SetTile(cellPosition, tile);
            }

            return layout;
        }

        public void Cleanup()
        {
            //noop
        }
    }
}
