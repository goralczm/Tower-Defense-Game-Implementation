using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class PathGenerationOrchestrator
{
    public class OnPathGeneratedEventArgs : EventArgs
    {
        public Vector2 StartPointWorld;
        public Vector2 EndPointWorld;
        public Bounds Bounds;
        public PathPreset PathPreset;
        public GenerationData GenerationData;
    }
    
    [SerializeField] private PathRenderer _pathRenderer;
    [SerializeField] private WaypointsParent _waypointsParent;
    [SerializeField] private GenerationData _generationData;

    private WaypointsExtractor _waypointsExtractor;
    private PathGenerator _pathGenerator;
    private Vector2 _startPosition;
    private int _seed;
    
    public static EventHandler OnPathGenerationStarted;
    public static EventHandler<OnPathGeneratedEventArgs> OnPathGenerationEnded;

    public Action<int> OnSeedChanged;
    
    public void SetSeed(int seed)
    {
        _seed = seed;
        OnSeedChanged?.Invoke(seed);
    }

    public void SetTilemapSettings(TilemapSettings tilemapSettings)
    {
        _pathRenderer.SetTilemapSettings(tilemapSettings);
        _waypointsExtractor.SetTilemapSettings(tilemapSettings);
    }

    public void SetTilemap(Tilemap tilemap)
    {
        _pathRenderer.SetTilemap(tilemap);
        _waypointsExtractor.SetTilemap(tilemap);
    }

    public void SetStartPoint(Vector2Int startPoint) => _generationData.StartPoint = startPoint;
    public void SetEndPoint(Vector2Int endPoint) => _generationData.EndPoint = endPoint;
    
    public async Task GeneratePath(PathPreset pathPreset, bool randomizeAccessPoints = false, bool enforceRules = true)
    {
        OnPathGenerationStarted?.Invoke(this, EventArgs.Empty);
        
        InitializeGenerators(pathPreset, randomizeAccessPoints);
        MazeLayoutGenerator layout = _pathGenerator.GeneratePath(enforceRules: enforceRules);
        await _pathRenderer.RenderPath(layout);
        ExtractAndCacheWaypoints();
        
        NotifyGenerationCompleted(pathPreset);
    }

    private void InitializeGenerators(PathPreset pathPreset, bool randomizeAccessPoints = false)
    {
        _generationData.MazeGenerationSettings = pathPreset.MazeGenerationSettings;
        _generationData.Seed = _seed;
        _generationData.OnSeedChanged = null;
        _generationData.OnSeedChanged += SetSeed;
        
        if (randomizeAccessPoints)
            _generationData.RandomizeAccessPoints();
        
        _pathGenerator = new PathGenerator(pathPreset.PathSettings, _generationData, _startPosition);
        
        Vector2Int entranceDir = GetAccessPointDir(_generationData.StartPoint);
        Vector2Int exitDir = -GetAccessPointDir(_generationData.EndPoint);
        
        _pathRenderer.SetPathSettings(pathPreset.PathSettings);
        _pathRenderer.SetGenerationData(_generationData);
        _pathRenderer.SetEntranceDir(entranceDir);
        _pathRenderer.SetExitDir(exitDir);
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
            Bounds = _pathRenderer.GetBounds(),
            PathPreset = pathPreset,
            GenerationData = _generationData
        });
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
