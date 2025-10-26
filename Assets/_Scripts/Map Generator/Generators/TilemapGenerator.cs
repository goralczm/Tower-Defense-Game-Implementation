using MapGenerator.Core;
using MapGenerator.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utilities.Extensions;

namespace MapGenerator.Generators
{
    [System.Serializable]
    public class TilemapGenerator : IGenerator
    {
        [Header("Debug")]
        [SerializeField] private bool _debug = true;
        [SerializeField] private int _delayInMiliseconds = 0;

        private TilemapSettings _tilemapSettings;
        private Tilemap _tilemap;
        private CancellationTokenSource _cts;

        public event Action<string> OnStatusChanged;

        public bool ShowDebug => _debug;
        public List<Type> RequiredGenerators => new();

        public TilemapGenerator(TilemapSettings tilemapSettings, Tilemap tilemap)
        {
            _tilemapSettings = tilemapSettings;
            _tilemap = tilemap;
        }

        public async Task<MapLayout> Generate(MapLayout layout, CancellationTokenSource cts)
        {
            _cts = cts;

            OnStatusChanged?.Invoke("Clearing tilemap...");
            _tilemap.ClearAllTiles();

            OnStatusChanged?.Invoke("Setting new tiles...");

            foreach (var node in layout.GetNodes())
            {
                _cts.Token.ThrowIfCancellationRequested();

                Tile tile = _tilemapSettings.GetTileByType(node.Type);
                if (tile == null) continue;

                Vector3Int cellPosition = node.GetPosition().ToVector3Int();

                _tilemap.SetTile(cellPosition, tile);
                await Task.Delay(_delayInMiliseconds);
            }

            return layout;
        }

        public void Cleanup()
        {
            //noop
        }

        public void DrawGizmos(DebugConfig debugConfig)
        {
            //noop
        }
    }
}
