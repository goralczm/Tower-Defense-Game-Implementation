#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.Tilemaps;
using Utilities.Extensions;
#endif

namespace MapGenerator.Core
{
#if UNITY_EDITOR
    public enum EnvironmentDebugView
    {
        None,
        Heatmap,
        Noise,
        Obstacles
    }

    [System.Serializable]
    public class DebugConfig
    {
        [HideInInspector] public Tilemap Tilemap;
        [HideInInspector] public MapLayout Layout;
        public Vector2 Offset;
        public EnvironmentDebugView EnvironmentDebugView;

        public Vector2 GetPositionOnTilemap(Vector2Int tilePos)
        {
            return ((Vector2)tilePos).Add(Tilemap.transform.position).Add(Offset);
        }
    }
#endif

    public interface IGenerator
    {
        public MapLayout Generate(MapLayout layout);

        public void Cleanup();

#if UNITY_EDITOR
        public void DrawGizmos(DebugConfig debugConfig);
#endif
    }
}
