using System;
using UnityEngine;

public class PathGenerationDirector : MonoBehaviour
{
    public class OnPathGeneratedEventArgs : EventArgs
    {
        public Vector2 StartPointWorld;
        public Vector2 EndPointWorld;
    }
    
    [Header("Path Settings")]
    [SerializeField] private PathSettings _pathSettings;

    [SerializeField] private GenerationData _generationData;
    
    [Header("Instances")]
    [SerializeField] private PathGenerator _pathGenerator;
    [SerializeField] private PathDisplay _pathDisplay;
    [SerializeField] private WaypointsExtractor _waypointsExtractor;

    public static EventHandler<OnPathGeneratedEventArgs> OnPathGenerated;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            GenerateWithNewSeed();
        
        if (Input.GetKeyDown(KeyCode.Space))
            RegeneratePath();
    }

    public void GenerateWithNewSeed()
    {
        _generationData.Seed = UnityEngine.Random.Range(-100000, 100000);
        _generationData.RandomizeStartAndEndPoints();
        
        RegeneratePath();
    }
    
    public void RegeneratePath()
    {
        _pathGenerator.SetGenerationData(_generationData);
        _pathDisplay.SetGenerationData(_generationData);
        _pathGenerator.SetPathSettings(_pathSettings);
        _pathDisplay.SetPathSettings(_pathSettings);
        
        MazeGenerator generator = _pathGenerator.GeneratePath();

        SetEntranceDirection();
        SetExitDirection();
        
        _pathDisplay.GenerateTilemap(generator);
        
        _waypointsExtractor.CacheRules();
        _waypointsExtractor.SetStartPoint(_pathDisplay.GetStartPointWorld());
        _waypointsExtractor.ExtractWaypoints();
        
        OnPathGenerated?.Invoke(this, new OnPathGeneratedEventArgs
        {
            StartPointWorld = _pathDisplay.GetStartPointWorld(),
            EndPointWorld = _pathDisplay.GetEndPointWorld()
        });
    }

    private void SetEntranceDirection()
    {
        Vector2Int entranceDir = Vector2Int.right;
        if (_generationData.StartPoint.y == 0)
            entranceDir = Vector2Int.up;
        else if (_generationData.StartPoint.y == _generationData.GenerationDataBase.Height - 1)
            entranceDir = Vector2Int.down;
        
        _pathDisplay.SetEntranceDirection(entranceDir);
    }
    
    private void SetExitDirection()
    {
        Vector2Int exitDir = Vector2Int.right;
        if (_generationData.EndPoint.y == 0)
            exitDir = Vector2Int.down;
        else if (_generationData.EndPoint.y == _generationData.GenerationDataBase.Height - 1)
            exitDir = Vector2Int.up;
        
        _pathDisplay.SetExitDirection(exitDir);
    }

    private void OnValidate()
    {
        _pathGenerator.SetPathSettings(_pathSettings);
        _pathDisplay.SetPathSettings(_pathSettings);
        _pathGenerator.SetGenerationData(_generationData);
        _pathDisplay.SetGenerationData(_generationData);
    }
}
