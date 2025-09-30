using MapGenerator.Core;
using MapGenerator.Generators;
using MapGenerator.Settings;
using MapGenerator.Utilities;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utilities.Extensions;

namespace MapGenerator.Demo
{
    public class MapBuilder
    {
        private List<IGenerator> _generators = new();

        private PathPreset _pathPreset;
        private GenerationConfig _generationConfig;
        private bool _enforceRules;
        private MapLayout _layout;

        public MapBuilder(PathPreset pathPreset, GenerationConfig generationConfig, bool enforceRules)
        {
            _pathPreset = pathPreset;
            _generationConfig = generationConfig;
            _enforceRules = enforceRules;

            WithGenerator(
                new PathLayoutGenerator(_pathPreset.PathSettings, _generationConfig, _enforceRules)
            );
        }

#if UNITY_EDITOR
        public MapLayout GetMapLayout() => _layout;
#endif

        public MapBuilder WithGenerator(IGenerator generator)
        {
            _generators.Add(generator);

            return this;
        }

        public void Build()
        {
            _layout = new();

            foreach (var generator in _generators)
                _layout = generator.Generate(_layout);
        }

        public void Cleanup()
        {
            foreach (var generator in _generators)
                generator.Cleanup();
        }
    }

    public class MapGenerator : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private PathPreset _pathPreset;
        [SerializeField] private GenerationConfig _generationConfig;
        [SerializeField] private TilemapSettings _tilemapSettings;
        
        [Header("References")]
        [SerializeField] private Tilemap _tilemap;
        [SerializeField] private List<GameObject> _obstaclePrefabs;

        [Header("Debug")]
        [SerializeField] private bool _debug;
        [SerializeField] private Vector2 _offset;

        private MapBuilder _mapBuilder;

        public GenerationConfig GetGenerationConfig() => _generationConfig;

        public void SetGenerationConfig(GenerationConfig generationConfig)
        {
            _generationConfig = generationConfig;
            OnValidate();
        }

        private void OnValidate()
        {
            if (_pathPreset)
                _generationConfig.MazeGenerationSettings = _pathPreset.MazeGenerationSettings;
            else
                Debug.LogWarning("Path Preset is not set!");

            _generationConfig.OnValidate();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                if (Input.GetKey(KeyCode.LeftShift))
                    Randomize();

                GenerateMap();
            }
        }

        public void GenerateMap()
        {
            _mapBuilder?.Cleanup();

            _mapBuilder = new MapBuilder(_pathPreset, _generationConfig, _generationConfig.EnforceRules);

            _mapBuilder
                .WithGenerator(new PathGenerator(_generationConfig, _generationConfig.RenderOverflowTiles))
                .WithGenerator(new RoundaboutsGenerator(_pathPreset.PathSettings, _generationConfig))
                .WithGenerator(new TilemapGenerator(_tilemap, _tilemapSettings))
                .WithGenerator(new EnvironmentGenerator(_pathPreset.EnvironmentSettings, _pathPreset.PathSettings, _generationConfig, _tilemap, _obstaclePrefabs))
                .WithGenerator(new WaypointsGenerator(_tilemapSettings, _tilemap))
                .Build();
        }

        private void Randomize()
        {
            _generationConfig.SetSeed(Randomizer.GetRandomSeed());
            _generationConfig.RandomizeAccessPoints();
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!_debug) return;

            DrawBoundingBoxGizmos();
            DrawAccessPointsGizmos();

            if (_mapBuilder == null || _mapBuilder.GetMapLayout() == null || _tilemap == null) return;

            DrawNodesGizmos();
            DrawPathArrowsGizmos();
        }

        private void DrawNodesGizmos()
        {
            foreach (var node in _mapBuilder.GetMapLayout().GetNodes())
            {
                if (node.Type == NodeType.Empty)
                    Gizmos.color = Color.red;
                else
                    Gizmos.color = Color.green;

                Gizmos.DrawWireSphere(GetPositionOnTilemap(node.GetPosition()), .2f);
            }
        }

        private Vector2 GetPositionOnTilemap(Vector2Int tilePos)
        {
            return ((Vector2)tilePos).Add(_tilemap.transform.position).Add(_offset);
        }

        public static void DrawGizmosArrow(Vector3 start, Vector3 end, Color color, float headLength = 0.2f, float headAngle = 20f)
        {
            Gizmos.color = color;
            Gizmos.DrawLine(start, end);

            Vector3 direction = (end - start).normalized;
            Vector3 right = Quaternion.Euler(0, 0, headAngle) * -direction;
            Vector3 left = Quaternion.Euler(0, 0, -headAngle) * -direction;

            Gizmos.DrawLine(end, end + right * headLength);
            Gizmos.DrawLine(end, end + left * headLength);
        }

        private void DrawBoundingBoxGizmos()
        {
            Vector3 size = new(_pathPreset.MazeGenerationSettings.Width,
                _pathPreset.MazeGenerationSettings.Height,
                0);

            Vector3 position = _tilemap.transform.position + size / 2f;

            if (_generationConfig.RenderOverflowTiles)
                size += Vector3.one * 2f;

            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(position, size);
        }

        private void DrawAccessPointsGizmos()
        {
            Vector3Int startCell = new(_generationConfig.GridStartPoint.x, _generationConfig.GridStartPoint.y, 0);
            Vector3Int endCell = new(_generationConfig.GridEndPoint.x, _generationConfig.GridEndPoint.y, 0);

            Gizmos.color = Color.magenta;
            Handles.Label(_tilemap.GetCellCenterWorld(startCell) + Vector3.up * 0.5f, "Start Point");
            Handles.Label(_tilemap.GetCellCenterWorld(endCell) + Vector3.up * 0.5f, "End Point");
            Gizmos.DrawWireSphere(_tilemap.GetCellCenterWorld(startCell), .2f);
            Gizmos.DrawWireSphere(_tilemap.GetCellCenterWorld(endCell), .2f);
        }

        private void DrawPathArrowsGizmos()
        {
            Gizmos.color = Color.magenta;
            var curr = _mapBuilder.GetMapLayout().GetByCoords(_generationConfig.GridStartPoint);
            DrawGizmosArrow(GetPositionOnTilemap(curr.GetPosition()), GetPositionOnTilemap(curr.Next.GetPosition()), Color.magenta);

            while (curr.Next != null)
            {
                curr = curr.Next;
                if (curr.Next != null)
                    DrawGizmosArrow(GetPositionOnTilemap(curr.GetPosition()), GetPositionOnTilemap(curr.Next.GetPosition()), Color.magenta);
            }
        }
#endif
    }
}
