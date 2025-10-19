#if UNITY_EDITOR
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utilities.Extensions;
#endif

namespace MapGenerator.Core
{
#if UNITY_EDITOR
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
#endif

    public interface IGenerator
    {
        public Task<MapLayout> Generate(MapLayout layout, CancellationTokenSource cts);

        public void Cleanup();

#if UNITY_EDITOR
        public void DrawGizmos(DebugConfig debugConfig);

        public bool ShowDebug { get; }
#endif
        public event Action<string> OnStatusChanged;
    }
}
