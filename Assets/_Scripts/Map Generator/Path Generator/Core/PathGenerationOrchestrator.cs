using System;
using System.Threading.Tasks;
using UnityEngine;

public class PathGenerationOrchestrator : MonoBehaviour
{
    public class OnPathGeneratedEventArgs : EventArgs
    {
        public Vector2 StartPointWorld;
        public Vector2 EndPointWorld;
        public Bounds Bounds;
        public PathPreset PathPreset;
        public GenerationData GenerationData;
    }

    [Header("Path Settings")]
    [SerializeField] private PathPreset _pathPreset;

    [Header("Path Settings")]
    [SerializeField] private PathSettings _pathSettings;

    [Header("Generation Data")]
    [SerializeField] private GenerationData _generationData;
    
    [Header("Instances")]
    [SerializeField] private PathGenerator _pathGenerator;
    [SerializeField] private PathRenderer _pathRenderer;
    [SerializeField] private WaypointsExtractor _waypointsExtractor;
    [SerializeField] private WaypointsParent _waypointsParent;

    public static EventHandler OnPathGenerationStarted;
    public static EventHandler<OnPathGeneratedEventArgs> OnPathGenerationEnded;

    private async void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            await GenerateWithNewSeed();
        
        if (Input.GetKeyDown(KeyCode.Space))
            await RegeneratePath();
    }

    public async Task GenerateWithNewSeed()
    {
        _generationData.Seed = UnityEngine.Random.Range(-100000, 100000);
        _generationData.RandomizeStartAndEndPoints();
        
        await RegeneratePath();
    }
    
    public async Task RegeneratePath()
    {
        OnPathGenerationStarted?.Invoke(this, EventArgs.Empty);
        
        _pathGenerator.SetGenerationData(_generationData);
        _pathRenderer.SetGenerationData(_generationData);
        _pathGenerator.SetPathSettings(_pathSettings);
        _pathRenderer.SetPathSettings(_pathSettings);

        MazeLayoutGenerator layoutGenerator = _pathGenerator.GeneratePath();

        _pathRenderer.SetEntranceDirection(GetAccessPointDir(_generationData.StartPoint));
        _pathRenderer.SetExitDirection(-GetAccessPointDir(_generationData.EndPoint));
        
        await _pathRenderer.GenerateTilemap(layoutGenerator);
        
        _waypointsExtractor.CacheRules();
        _waypointsExtractor.SetStartPoint(_pathRenderer.GetStartPointWorld());
        _waypointsExtractor.ExtractWaypoints();

        _waypointsParent.CacheWaypoints(_pathRenderer.GetStartPointWorld());
        
        OnPathGenerationEnded?.Invoke(this, new OnPathGeneratedEventArgs
        {
            StartPointWorld = _pathRenderer.GetStartPointWorld(),
            EndPointWorld = _pathRenderer.GetEndPointWorld(),
            Bounds = _pathRenderer.GetBounds(),
            PathPreset = _pathPreset,
            GenerationData = _generationData
        });
    }

    private Vector2Int GetAccessPointDir(Vector2Int accessPoint)
    {
        Vector2Int accessDir = accessPoint.x == 0 ? Vector2Int.right : Vector2Int.left;

        if (accessPoint.y == 0)
            accessDir = Vector2Int.up;
        else if (accessPoint.y == _generationData.GenerationDataBase.Height - 1)
            accessDir = Vector2Int.down;

        return accessDir;
    }

    public void SetPathPreset(PathPreset pathPreset)
    {
        _pathPreset = pathPreset;
        OnValidate();
    }

    private void OnValidate()
    {
        if (_pathPreset != null)
        {
            _pathSettings = _pathPreset.PathSettings;
            _generationData.GenerationDataBase = _pathPreset.GenerationDataBase;
        }

        _pathGenerator.SetPathSettings(_pathSettings);
        _pathRenderer.SetPathSettings(_pathSettings);
        _pathGenerator.SetGenerationData(_generationData);
        _pathRenderer.SetGenerationData(_generationData);
    }
}
