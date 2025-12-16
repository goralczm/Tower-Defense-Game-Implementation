using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utilities.Extensions;

namespace MapGenerator.Core
{
    [System.Serializable]
    public class DebugConfig
    {
        [HideInInspector] public Tilemap Tilemap;
        [HideInInspector] public MapLayout Layout;
        public Vector2 Offset;

        public Vector2 GetPositionOnTilemap(Vector2Int tilePos)
        {
            return ((Vector2)tilePos).Add(Tilemap.transform.position).Add(Offset);
        }
    }

    public interface IGenerator
    {
        public List<Type> RequiredGenerators { get; }
        
        public Task<MapLayout> Generate(MapLayout layout, CancellationTokenSource cts);

        public void Cleanup();

        public void DrawGizmos(DebugConfig debugConfig);

        public bool ShowDebug { get; }

        public event Action<string> OnStatusChanged;
    }
}
