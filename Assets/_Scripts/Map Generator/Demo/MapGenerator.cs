using ArtificeToolkit.Attributes;
using MapGenerator.Core;
using MapGenerator.Generators;
using MapGenerator.Settings;
using MapGenerator.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
#if UNITY_EDITOR
using UnityEditor;
#endif
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
        private MapLayout _layout;
        private CancellationTokenSource _cts;
        private bool _enforceRules;

        public event Action<string> OnStatusChanged;

        public MapBuilder(PathPreset pathPreset, GenerationConfig generationConfig, bool enforceRules)
        {
            _pathPreset = pathPreset;
            _generationConfig = generationConfig;
            _enforceRules = enforceRules;

            WithGenerator(
                new PathLayoutGenerator(_pathPreset.PathSettings, _generationConfig, _enforceRules)
            );
        }

        public MapLayout GetMapLayout() => _layout;

        public List<IGenerator> GetGenerators() => _generators;

        public MapBuilder WithGenerator(IGenerator generator)
        {
            var generatorTypes = _generators.Select(g => g.GetType()).ToList();
            foreach (var requiredGenerator in generator.RequiredGenerators)
            {
                if (!generatorTypes.Contains(requiredGenerator))
                {
                    Debug.LogError($"{requiredGenerator} must be added before {generator.GetType()}");
                    return this;
                }
            }

            generator.OnStatusChanged += OnStatusChanged;
            _generators.Add(generator);

            return this;
        }

        public async Task BuildAsync()
        {
            _layout = new();

            _cts = new();

            OnStatusChanged?.Invoke("Generating path...");

            foreach (var generator in _generators)
            {
                _cts.Token.ThrowIfCancellationRequested();

                _layout = await generator.Generate(_layout, _cts);
                await Task.Delay(100);
            }
        }

        public void Cleanup()
        {
            foreach (var generator in _generators)
            {
                generator.Cleanup();
                generator.OnStatusChanged -= OnStatusChanged;
            }
        }

        public void CancelBuild()
        {
            _cts?.Cancel();
        }
    }

    public class MapGenerator : MonoBehaviour
    {
        public class OnMapGeneratedEventArgs : EventArgs
        {
            public Vector2 StartPoint;
            public Vector2 EndPoint;
        }

        [Header("Settings")]
        [SerializeField] private PathPreset _pathPreset;
        [SerializeField] private GenerationConfig _generationConfig;
        [SerializeField] private TilemapSettings _tilemapSettings;

        [Header("References")]
        [SerializeField] private Tilemap _tilemap;

        [Header("Debug")]
        [SerializeField] private bool _debug;
        [SerializeField] private DebugConfig _debugConfig;

        [SerializeField, SerializeReference, ForceArtifice] private List<IGenerator> _generators = new();

        private MapBuilder _mapBuilder;

        public event EventHandler OnMapGenerationStarted;
        public event EventHandler OnMapGenerationCanceled;
        public event EventHandler<OnMapGeneratedEventArgs> OnMapGenerationEnded;
        public event Action<string> OnStatusChanged;

        public GenerationConfig GetGenerationConfig() => _generationConfig;

        public void SetGenerationConfig(GenerationConfig generationConfig)
        {
            _generationConfig = generationConfig;
            OnValidate();
        }

        public void OnValidate()
        {
            if (_pathPreset)
                _generationConfig.MazeGenerationSettings = _pathPreset.MazeGenerationSettings;
            else
                Debug.LogWarning("Path Preset is not set!");

            _generationConfig.OnValidate();
        }

        public async Task GenerateMapAsync()
        {
            PrepareBuilder();

            await ContinueBuilderAsync();
        }

        public void PrepareBuilder()
        {
            _mapBuilder?.Cleanup();

            OnMapGenerationStarted?.Invoke(this, EventArgs.Empty);

            _mapBuilder = new MapBuilder(_pathPreset, _generationConfig, _generationConfig.EnforceRules);
            _mapBuilder.OnStatusChanged += OnStatusChanged;

            _mapBuilder
                .WithGenerator(new PathGenerator(_pathPreset.MazeGenerationSettings, _generationConfig, _generationConfig.RenderOverflowTiles))
                .WithGenerator(new RoundaboutsGenerator(_pathPreset.PathSettings, _generationConfig))
                .WithGenerator(new TilemapGenerator(_tilemapSettings, _tilemap))
                .WithGenerator(new EnvironmentGenerator(_pathPreset.EnvironmentSettings, _pathPreset.PathSettings, _generationConfig, _tilemap))
                .WithGenerator(new WaypointsGenerator(_tilemapSettings, _tilemap));

            _generators = _mapBuilder.GetGenerators();
        }

        public async Task ContinueBuilderAsync()
        {
            await _mapBuilder.BuildAsync();

            OnMapGenerationEnded?.Invoke(this, new OnMapGeneratedEventArgs
            {
                StartPoint = _tilemap.GetCellCenterWorld(_mapBuilder.GetMapLayout().StartPoint.ToVector3Int()),
                EndPoint = _tilemap.GetCellCenterWorld(_mapBuilder.GetMapLayout().EndPoint.ToVector3Int()),
            });
        }

        public void CancelBuild()
        {
            _mapBuilder?.CancelBuild();
            _mapBuilder?.Cleanup();
            OnMapGenerationCanceled?.Invoke(this, EventArgs.Empty);
        }

        public void RandomizeConfig()
        {
            _generationConfig.SetSeed(Randomizer.GetRandomSeed());
            _generationConfig.RandomizeAccessPoints();
        }

        private Bounds GetBoundingBox()
        {
            Vector3 size = new(_pathPreset.MazeGenerationSettings.Width,
                _pathPreset.MazeGenerationSettings.Height,
                0);

            Vector3 position = _tilemap.transform.position + size / 2f;

            if (_generationConfig.RenderOverflowTiles)
                size += Vector3.one * 2f;

            return new Bounds(position, size);
        }

        private void OnApplicationQuit()
        {
            _mapBuilder?.CancelBuild();
        }

        private void OnDrawGizmos()
        {
            if (!_debug) return;

            DrawBoundingBoxGizmos();
            DrawAccessPointsGizmos();
            DrawMiddlePointsGizmos();

            if (_mapBuilder == null || _mapBuilder.GetMapLayout() == null || _tilemap == null) return;

            _debugConfig.Layout = _mapBuilder.GetMapLayout();
            _debugConfig.Tilemap = _tilemap;

            foreach (var generator in _generators)
            {
                if (!generator.ShowDebug) continue;

                generator.DrawGizmos(_debugConfig);
            }
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
            Bounds boundingBox = GetBoundingBox();

            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(boundingBox.center, boundingBox.size);
        }

        private void DrawAccessPointsGizmos()
        {
            Vector3Int startCell = new(_generationConfig.GridStartPoint.x, _generationConfig.GridStartPoint.y, 0);
            Vector3Int endCell = new(_generationConfig.GridEndPoint.x, _generationConfig.GridEndPoint.y, 0);

            Gizmos.color = Color.magenta;
#if UNITY_EDITOR
            Handles.Label(_tilemap.GetCellCenterWorld(startCell) + Vector3.up * 0.5f, "Start Point");
            Handles.Label(_tilemap.GetCellCenterWorld(endCell) + Vector3.up * 0.5f, "End Point");
#endif
            Gizmos.DrawWireSphere(_tilemap.GetCellCenterWorld(startCell), .2f);
            Gizmos.DrawWireSphere(_tilemap.GetCellCenterWorld(endCell), .2f);
        }

        private void DrawMiddlePointsGizmos()
        {
            Gizmos.color = Color.yellow;
            int i = 0;
            foreach (var middlePoint in _pathPreset.MazeGenerationSettings.MiddlePoints)
            {
                Vector2 middlePointWorldPos = _tilemap.GetCellCenterWorld(middlePoint.ToVector3Int());
                Gizmos.DrawWireSphere(middlePointWorldPos, .2f);
#if UNITY_EDITOR
                Handles.Label(middlePointWorldPos.Add(y: .5f), $"Middle Point {i}");
#endif
                i++;
            }
        }
    }
}
