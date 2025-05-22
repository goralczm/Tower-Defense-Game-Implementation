using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GenerationData
{
    public int Width = 5;
    public int Height = 5;
    public int Seed;
    public Vector2Int StartPoint = new(0, 0);
    public List<Vector2Int> MiddlePoints = new();
    public Vector2Int EndPoint = new(5, 5);
    
    public void RandomizeStartAndEndPoints()
    {
        Vector2Int startPoint = new Vector2Int(0, UnityEngine.Random.Range(0, Height));
        Vector2Int endPoint = new Vector2Int(Width - 1, UnityEngine.Random.Range(0, Height));

        StartPoint = startPoint;
        EndPoint = endPoint;
    }

    public void OnValidate()
    {
        Width = Mathf.Max(2, Width);
        Height = Mathf.Max(2, Height);
        StartPoint = ClampToBounds(StartPoint);
        EndPoint = ClampToBounds(EndPoint);
    }
    
    private Vector2Int ClampToBounds(Vector2Int pos)
    {
        return new(Mathf.Clamp(pos.x, 0, Width - 1), Mathf.Clamp(pos.y, 0, Height - 1));
    }
}

public class PathGenerationDirector : MonoBehaviour
{
    public class OnPathGeneratedEventArgs : EventArgs
    {
        public Vector2 StartPoint;
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
        _pathGenerator.SetGenerationData(_generationData);
        _pathDisplay.SetGenerationData(_generationData);
        RegeneratePath();
    }
    
    public void RegeneratePath()
    {
        _pathGenerator.SetPathSettings(_pathSettings);
        _pathDisplay.SetPathSettings(_pathSettings);
        
        MazeGenerator generator = _pathGenerator.GeneratePath();

        SetEntranceDirection();
        SetExitDirection();
        
        _pathDisplay.GenerateTilemap(generator);
        
        _waypointsExtractor.CacheRules();
        _waypointsExtractor.SetStartPoint(_pathDisplay.GetStartPoint());
        _waypointsExtractor.ExtractWaypoints();
        
        OnPathGenerated?.Invoke(this, new OnPathGeneratedEventArgs
        {
            StartPoint = _pathDisplay.GetStartPoint()
        });
    }

    private void SetEntranceDirection()
    {
        Vector2Int entranceDir = Vector2Int.right;
        if (_generationData.StartPoint.y == 0)
            entranceDir = Vector2Int.up;
        else if (_generationData.StartPoint.y == _generationData.Height - 1)
            entranceDir = Vector2Int.down;
        
        _pathDisplay.SetEntranceDirection(entranceDir);
    }
    
    private void SetExitDirection()
    {
        Vector2Int exitDir = Vector2Int.right;
        if (_generationData.EndPoint.y == 0)
            exitDir = Vector2Int.down;
        else if (_generationData.EndPoint.y == _generationData.Height - 1)
            exitDir = Vector2Int.up;
        
        _pathDisplay.SetExitDirection(exitDir);
    }

    private void OnValidate()
    {
        _generationData.OnValidate();
    }
}
