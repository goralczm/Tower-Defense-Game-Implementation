using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class PathOrchestrator
{
    public class OnPathGeneratedEventArgs : EventArgs
    {
        public Vector2 StartPointWorld;
        public Vector2 EndPointWorld;
        public Vector2Int GridStartPoint;
        public Vector2Int GridEndPoint;
        public Bounds Bounds;
        public PathPreset PathPreset;
        public GenerationConfig GenerationData;
        public int Seed;
    }

    [Header("References")]
    [SerializeField] private WaypointsParent _waypointsParent;

    private GenerationConfig _generationData;
    private PathGenerator _pathGenerator;
    private PathRenderer _pathRenderer;
    private WaypointsExtractor _waypointsExtractor;
    private TilemapSettings _tilemapSettings;
    private Tilemap _pathTilemap;
    private Vector2 _startPosition;
    private Vector2Int _gridStartPoint;
    private Vector2Int _gridEndPoint;
    private int _seed;

    public EventHandler OnPathGenerationStarted;
    public EventHandler<OnPathGeneratedEventArgs> OnPathGenerationEnded;

    public Vector3 Center => _pathTilemap.transform.position;

    public Bounds GetPathBounds()
    {
        if (_generationData == null || _generationData.MazeGenerationSettings == null) return new Bounds();

        return new Bounds(
            new Vector3(_generationData.MazeGenerationSettings.Width / 2f,
                _generationData.MazeGenerationSettings.Height / 2f, 0) + Center,
            new Vector3(_generationData.MazeGenerationSettings.Width + 1,
                _generationData.MazeGenerationSettings.Height + 1, 0));
    }

    public int GetSeed() => _seed;

    public void SetSeed(int seed)
    {
        _seed = seed;
    }

    public void SetTilemapSettings(TilemapSettings tilemapSettings)
    {
        _tilemapSettings = tilemapSettings;
    }

    public void SetTilemap(Tilemap tilemap)
    {
        _pathTilemap = tilemap;
    }

    public void SetGridStartPoint(Vector2Int gridStartPoint) => _gridStartPoint = gridStartPoint;
    public void SetGridEndPoint(Vector2Int gridEndPoint) => _gridEndPoint = gridEndPoint;

    public async Task GeneratePath(PathPreset pathPreset, bool randomizeAccessPoints = false, bool enforceRules = true,
        bool renderOverflowTiles = false)
    {
        OnPathGenerationEnded += ApplyGenerationChanges;
        OnPathGenerationStarted?.Invoke(this, EventArgs.Empty);

        InitializeGenerators(pathPreset, randomizeAccessPoints);
        MazeLayoutGenerator layout = _pathGenerator.GeneratePath(enforceRules: enforceRules);
        await _pathRenderer.RenderPath(layout, renderOverflowTiles);
        ExtractAndCacheWaypoints();

        NotifyGenerationCompleted(pathPreset);
        OnPathGenerationEnded -= ApplyGenerationChanges;
    }

    private void InitializeGenerators(PathPreset pathPreset, bool randomizeAccessPoints = false)
    {
        _generationData = new GenerationConfig(pathPreset.MazeGenerationSettings, _seed,
            _gridStartPoint, _gridEndPoint);

        if (randomizeAccessPoints)
            _generationData.RandomizeAccessPoints();

        _pathGenerator = new PathGenerator(pathPreset.PathSettings, _generationData, _startPosition);

        Vector2Int entranceDir = GetAccessPointDir(_generationData.GridStartPoint);
        Vector2Int exitDir = -GetAccessPointDir(_generationData.GridEndPoint);

        _pathRenderer = new PathRenderer(_pathTilemap, pathPreset.PathSettings, _tilemapSettings, _generationData,
            entranceDir, exitDir);

        _waypointsExtractor = new WaypointsExtractor(_pathTilemap, _tilemapSettings);
    }

    private void ExtractAndCacheWaypoints()
    {
        Vector3 startWorldPoint = _pathRenderer.GetStartPointWorld();

        _waypointsExtractor.CacheRules();
        List<Vector2> waypoints = _waypointsExtractor.ExtractWaypoints(startWorldPoint);
        _waypointsParent.CacheWaypoints(waypoints);
    }

    private void NotifyGenerationCompleted(PathPreset pathPreset)
    {
        OnPathGenerationEnded?.Invoke(this, new OnPathGeneratedEventArgs
        {
            StartPointWorld = _pathRenderer.GetStartPointWorld(),
            EndPointWorld = _pathRenderer.GetEndPointWorld(),
            Bounds = GetPathBounds(),
            PathPreset = pathPreset,
            GenerationData = _generationData,
            Seed = _generationData.Seed,
            GridStartPoint = _generationData.GridStartPoint,
            GridEndPoint = _generationData.GridEndPoint,
        });
    }

    private void ApplyGenerationChanges(object sender, OnPathGeneratedEventArgs e)
    {
        SetSeed(e.Seed);
    }

    private Vector2Int GetAccessPointDir(Vector2Int accessPoint)
    {
        Vector2Int accessDir = accessPoint.x == 0 ? Vector2Int.right : Vector2Int.left;

        if (accessPoint.y == 0)
            accessDir = Vector2Int.up;
        else if (accessPoint.y == _generationData.MazeGenerationSettings.Height - 1)
            accessDir = Vector2Int.down;

        return accessDir;
    }
}