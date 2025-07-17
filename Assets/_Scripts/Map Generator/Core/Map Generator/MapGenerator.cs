using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace _Scripts.Map_Generator.Core.Map
{
    public class MapGenerator : MonoBehaviour
    {
        private class MapBuilder
        {
            private readonly PathOrchestrator _pathOrchestrator;
            private readonly EnvironmentGenerator _environmentGenerator;

            private PathPreset _pathPreset;
            private TilemapSettings _tilemapSettings;
            private Tilemap _tilemap;
            private int _seed = 42;
            private bool _generateEnvironment = false;
            private bool _randomizeAccessPoints = false;
            private bool _enforceRules = false;
            private bool _renderOverflowTiles = false;
            private Vector2Int _gridStartPoint;
            private Vector2Int _gridEndPoint;

            public MapBuilder(PathOrchestrator orchestrator, EnvironmentGenerator environmentGenerator)
            {
                _pathOrchestrator = orchestrator ?? throw new ArgumentNullException(nameof(orchestrator));
                _environmentGenerator =
                    environmentGenerator ?? throw new ArgumentNullException(nameof(environmentGenerator));
            }

            public MapBuilder WithPreset(PathPreset preset)
            {
                _pathPreset = preset ?? throw new ArgumentNullException(nameof(preset));
                return this;
            }

            public MapBuilder WithTilemap(Tilemap tilemap)
            {
                _tilemap = tilemap ?? throw new ArgumentNullException(nameof(tilemap));
                return this;
            }

            public MapBuilder WithTilemapSettings(TilemapSettings tilemapSettings)
            {
                _tilemapSettings = tilemapSettings ?? throw new ArgumentNullException(nameof(tilemapSettings));
                return this;
            }

            public MapBuilder WithSeed(int seed)
            {
                _seed = seed;
                return this;
            }

            public MapBuilder RandomizeAccessPoints(bool randomizeAccessPoints)
            {
                _randomizeAccessPoints = randomizeAccessPoints;
                return this;
            }

            public MapBuilder IncludePathGenerationRules(bool enforceRules)
            {
                _enforceRules = enforceRules;
                return this;
            }

            public MapBuilder WithOverflowTiles(bool renderOverflowTiles)
            {
                _renderOverflowTiles = renderOverflowTiles;
                return this;
            }

            public MapBuilder WithStartPoint(Vector2Int startPoint)
            {
                _gridStartPoint = startPoint;
                return this;
            }

            public MapBuilder WithEndPoint(Vector2Int endPoint)
            {
                _gridEndPoint = endPoint;
                return this;
            }

            public MapBuilder IncludeEnvironment(bool generateEnvironment = true)
            {
                _generateEnvironment = generateEnvironment;
                return this;
            }

            public async Task BuildAsync()
            {
                ValidateConfiguration();

                try
                {
                    SubscribeEvents();
                    ApplyConfiguration();
                    await _environmentGenerator.DestroyAllObstacles();
                    await _pathOrchestrator.GeneratePath(_pathPreset, _randomizeAccessPoints, _enforceRules, _renderOverflowTiles);
                    if (_generateEnvironment)
                        await _environmentGenerator.GenerateEnvironment(_pathPreset, _pathOrchestrator.GetPathBounds(), _seed);
                }
                finally
                {
                    UnsubscribeEvents();
                }
            }

            private void SubscribeEvents()
            {
                _pathOrchestrator.OnPathGenerationEnded += UpdateConfigurationAfterGeneration;
            }

            private void UnsubscribeEvents()
            {
                _pathOrchestrator.OnPathGenerationEnded -= UpdateConfigurationAfterGeneration;
            }

            private void ApplyConfiguration()
            {
                _pathOrchestrator.SetSeed(_seed);
                _pathOrchestrator.SetTilemap(_tilemap);
                _pathOrchestrator.SetGridStartPoint(_gridStartPoint);
                _pathOrchestrator.SetGridEndPoint(_gridEndPoint);
                _pathOrchestrator.SetTilemapSettings(_tilemapSettings);
                
                if (_generateEnvironment)
                    _environmentGenerator.SetTilemap(_tilemap);
            }

            private void UpdateConfigurationAfterGeneration(object sender, PathOrchestrator.OnPathGeneratedEventArgs e)
            {
                _seed = e.Seed;
            }

            private void ValidateConfiguration()
            {
                if (_pathPreset == null) throw new InvalidOperationException("Path Preset must be specified.");
                if (_tilemap == null) throw new InvalidOperationException("Tilemap must be specified.");
                if (_tilemapSettings == null) throw new InvalidOperationException("Tilemap Settings must be specified.");
            }
        }

        [Header("Settings")]
        [SerializeField] private MapGenerationContext _context;
        [SerializeField] private TilemapSettings _tilemapSettings;
        [SerializeField] private bool _includePathGenerationRules = true;
        [SerializeField] private bool _randomizeAccessPoints = true;
        [SerializeField] private bool _renderOverflowTiles = true;

        [Header("References")]
        [SerializeField] private Tilemap _mapTilemap;

        [Header("Debug")]
        [SerializeField] private bool _debug;
        [SerializeField] private Color _accessPointsColor = Color.red;
        [SerializeField] private float _accessPointsRadius = .2f;       

        [SerializeField] private PathOrchestrator _pathOrchestrator;
        [SerializeField] private EnvironmentGenerator _environmentGenerator;

        private MapBuilder _builder;

        public Action OnMapGenerationStarted;
        public Action OnMapGenerationEnded;

        public void SetSeed(int seed)
        {
            _context.Seed = seed;
        }

        public void SetPathPreset(PathPreset pathPreset)
        {
            _context.PathPreset = pathPreset;
        }

        public void SetStartPoint(Vector2Int gridStartPoint)
        {
            _context.GridStartPoint = gridStartPoint;
        }

        public void SetEndPoint(Vector2Int gridEndPoint)
        {
            _context.GridEndPoint = gridEndPoint;
        }

        public MapGenerationContext GetGenerationContext()
        {
            return _context;
        }
        
        public void SetGenerationContext(MapGenerationContext context)
        {
            _context = context;
        }
        
        public Tilemap GetTilemap() => _mapTilemap;

        private void ApplyGenerationChanges(object sender, PathOrchestrator.OnPathGeneratedEventArgs e)
        {
            SetSeed(e.Seed);
            SetStartPoint(e.GridStartPoint);
            SetEndPoint(e.GridEndPoint);
        }

        public async Task GenerateMapByContext()
        {
            _builder = new MapBuilder(_pathOrchestrator, _environmentGenerator)
                .WithPreset(_context.PathPreset)
                .WithTilemap(_mapTilemap)
                .WithTilemapSettings(_tilemapSettings)
                .WithSeed(_context.Seed)
                .WithStartPoint(_context.GridStartPoint)
                .WithEndPoint(_context.GridEndPoint)
                .RandomizeAccessPoints(false)
                .IncludePathGenerationRules(false)
                .IncludeEnvironment(_context.GenerateEnvironment)
                .WithOverflowTiles(_renderOverflowTiles);

            await ProcessGeneration(_builder);
        }

        public async Task GenerateRandomMap()
        {
            _builder = new MapBuilder(_pathOrchestrator, _environmentGenerator)
                .WithPreset(_context.PathPreset)
                .WithTilemap(_mapTilemap)
                .WithTilemapSettings(_tilemapSettings)
                .WithSeed(_context.Seed)
                .WithStartPoint(_context.GridStartPoint)
                .WithEndPoint(_context.GridEndPoint)
                .RandomizeAccessPoints(_randomizeAccessPoints)
                .IncludePathGenerationRules(_includePathGenerationRules)
                .IncludeEnvironment(_context.GenerateEnvironment)
                .WithOverflowTiles(_renderOverflowTiles);

            await ProcessGeneration(_builder);
        }

        private async Task ProcessGeneration(MapBuilder mapBuilder)
        {
            OnMapGenerationStarted?.Invoke();
            _pathOrchestrator.OnPathGenerationEnded += ApplyGenerationChanges;

            await _builder.BuildAsync();
            
            _pathOrchestrator.OnPathGenerationEnded -= ApplyGenerationChanges;
            OnMapGenerationEnded?.Invoke();
        }

        private async void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                SetSeed(Randomizer.GetRandomSeed());
                await GenerateRandomMap();
            }

            if (Input.GetKeyDown(KeyCode.Space))
                await GenerateMapByContext();
        }
        
        private void OnDrawGizmos()
        {
            if (!_debug) return;
            
            _environmentGenerator.OnDrawGizmos();

            BoundingBoxGizmos();
            AccessPointsGizmos();
        }

        private void BoundingBoxGizmos()
        {
            Vector3 size = new(_context.PathPreset.MazeGenerationSettings.Width,
                _context.PathPreset.MazeGenerationSettings.Height,
                0);

            Vector3 position = _mapTilemap.transform.position + size / 2f;

            if (_renderOverflowTiles)
                size += Vector3.one * 2f;

            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(position, size);
        }

        private void AccessPointsGizmos()
        {
            Vector3Int startCell = new(_context.GridStartPoint.x, _context.GridStartPoint.y, 0);
            Vector3Int endCell = new(_context.GridEndPoint.x, _context.GridEndPoint.y, 0);
            
            Gizmos.color = _accessPointsColor;
#if UNITY_EDITOR
            Handles.Label(_mapTilemap.GetCellCenterWorld(startCell) + Vector3.up * 0.5f, "Start Point");
            Handles.Label(_mapTilemap.GetCellCenterWorld(endCell) + Vector3.up * 0.5f, "End Point");
#endif
            Gizmos.DrawWireSphere(_mapTilemap.GetCellCenterWorld(startCell), _accessPointsRadius);
            Gizmos.DrawWireSphere(_mapTilemap.GetCellCenterWorld(endCell), _accessPointsRadius);
        }

        private void OnValidate()
        {
            _context.GridStartPoint = new Vector2Int(
                Mathf.Clamp(_context.GridStartPoint.x, 0, _context.PathPreset.MazeGenerationSettings.Width - 1),
                Mathf.Clamp(_context.GridStartPoint.y, 0, _context.PathPreset.MazeGenerationSettings.Height - 1)
            );

            _context.GridEndPoint = new Vector2Int(
                Mathf.Clamp(_context.GridEndPoint.x, 0, _context.PathPreset.MazeGenerationSettings.Width - 1),
                Mathf.Clamp(_context.GridEndPoint.y, 0, _context.PathPreset.MazeGenerationSettings.Height - 1)
            );
        }
    }
}