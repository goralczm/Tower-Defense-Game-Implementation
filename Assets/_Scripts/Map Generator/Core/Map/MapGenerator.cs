using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Scripts.Map_Generator.Core.Map
{
    public class MapGenerator : MonoBehaviour
    {
        private class MapBuilder
        {
            private PathPreset _pathPreset;
            private Tilemap _tilemap;
            private int _seed = 42;
            private bool _generateEnvironment = false;
            private bool _randomizeAccessPoints = false;
            private bool _enforceRules = true;
            private Vector2Int _startPoint;
            private Vector2Int _endPoint;
            private TilemapSettings _tilemapSettings;

            private MapGenerator _mapGenerator;
            private PathGenerationOrchestrator _pathGenerationOrchestrator;
            private EnvironmentGenerator _environmentGenerator;

            public MapBuilder(MapGenerator mapGenerator, PathGenerationOrchestrator orchestrator, EnvironmentGenerator environmentGenerator)
            {
                _mapGenerator = mapGenerator;
                _pathGenerationOrchestrator = orchestrator;
                _environmentGenerator = environmentGenerator;
            }

            public MapBuilder WithPreset(PathPreset preset)
            {
                _pathPreset = preset;
                return this;
            }

            public MapBuilder WithSeed(int seed)
            {
                _seed = seed;
                return this;
            }

            public MapBuilder WithTilemap(Tilemap tilemap)
            {
                _tilemap = tilemap;
                return this;
            }

            public MapBuilder WithTilemapSettings(TilemapSettings tilemapSettings)
            {
                _tilemapSettings = tilemapSettings;
                return this;
            }

            public MapBuilder WithRandomizeAccessPoints(bool randomizeAccessPoints)
            {
                _randomizeAccessPoints = randomizeAccessPoints;
                return this;
            }

            public MapBuilder WithEnforceRules(bool enforceRules)
            {
                _enforceRules = enforceRules;
                return this;
            }

            public MapBuilder WithStartPoint(Vector2Int startPoint)
            {
                _startPoint = startPoint;
                return this;
            }
            
            public MapBuilder WithEndPoint(Vector2Int endPoint)
            {
                _endPoint = endPoint;
                return this;
            }

            public MapBuilder WithEnvironment(bool generateEnvironment = true)
            {
                _generateEnvironment = generateEnvironment;
                return this;
            }

            public async Task BuildAsync()
            {
                if (_pathPreset == null)
                {
                    Debug.LogError("Path Preset must be set before building the map.");
                    return;
                }
                
                if (_tilemap == null)
                {
                    Debug.LogError("Tilemap must be set before building the map.");
                    return;
                }

                if (_generateEnvironment)
                    PathGenerationOrchestrator.OnPathGenerationEnded += _environmentGenerator.CreateEnvironment;

                _pathGenerationOrchestrator.OnSeedChanged += _mapGenerator.SetSeed;
                
                _pathGenerationOrchestrator.SetSeed(_seed);
                _pathGenerationOrchestrator.SetTilemap(_tilemap);
                _pathGenerationOrchestrator.SetStartPoint(_startPoint);
                _pathGenerationOrchestrator.SetEndPoint(_endPoint);
                _pathGenerationOrchestrator.SetTilemapSettings(_tilemapSettings);
                _environmentGenerator.SetTilemap(_tilemap);
                await _pathGenerationOrchestrator.GeneratePath(_pathPreset, _randomizeAccessPoints, _enforceRules);

                if (_generateEnvironment)
                    PathGenerationOrchestrator.OnPathGenerationEnded -= _environmentGenerator.CreateEnvironment;
                
                _pathGenerationOrchestrator.OnSeedChanged -= _mapGenerator.SetSeed;
            }
        }
        
        [Header("Settings")]
        [SerializeField] private PathPreset _pathPreset;
        [SerializeField] private TilemapSettings _tilemapSettings;
        [SerializeField] private int _seed = 42;
        [SerializeField] private bool _generateEnvironment = true;
        [SerializeField] private bool _randomizeAccessPoints = false;
        [SerializeField] private bool _enforceRules = true;
        [SerializeField] private Vector2Int _startPoint;
        [SerializeField] private Vector2Int _endPoint;

        [Header("References")]
        [SerializeField] private Tilemap _mapTilemap;
        [SerializeField] private PathGenerationOrchestrator _pathGenerationOrchestrator;
        [SerializeField] private EnvironmentGenerator _environmentGenerator;

        private MapBuilder _builder;

        public void SetSeed(int seed)
        {
            _seed = seed;
        }

        public void SetPathPreset(PathPreset pathPreset)
        {
            _pathPreset = pathPreset;
        }
        
        public async Task GenerateMap()
        {
            _builder = new MapBuilder(this, _pathGenerationOrchestrator, _environmentGenerator)
                .WithPreset(_pathPreset)
                .WithTilemap(_mapTilemap)
                .WithTilemapSettings(_tilemapSettings)
                .WithSeed(_seed)
                .WithStartPoint(_startPoint)
                .WithEndPoint(_endPoint)
                .WithRandomizeAccessPoints(_randomizeAccessPoints)
                .WithEnforceRules(_enforceRules)
                .WithEnvironment(_generateEnvironment);
            
            await _builder.BuildAsync();
        }
        
        private async void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                SetSeed(Randomizer.GetRandomSeed());
                await GenerateMap();
            }

            if (Input.GetKeyDown(KeyCode.Space))
                await GenerateMap();
        }

        private void OnDrawGizmos()
        {
            _environmentGenerator.OnDrawGizmos();
        }

        private void OnValidate()
        {
            _startPoint = new Vector2Int(
                Mathf.Clamp(_startPoint.x, 0, _pathPreset.MazeGenerationSettings.Width - 1),
                Mathf.Clamp(_startPoint.y, 0, _pathPreset.MazeGenerationSettings.Height - 1)
            );
            
            _endPoint = new Vector2Int(
                Mathf.Clamp(_endPoint.x, 0, _pathPreset.MazeGenerationSettings.Width - 1),
                Mathf.Clamp(_endPoint.y, 0, _pathPreset.MazeGenerationSettings.Height - 1)
            );
        }
    }
}