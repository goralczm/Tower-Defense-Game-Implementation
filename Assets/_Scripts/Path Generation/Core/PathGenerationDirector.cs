using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class PathGenerationDirector : MonoBehaviour
{
    public class OnPathGeneratedEventArgs : EventArgs
    {
        public Vector2 StartPointWorld;
        public Vector2 EndPointWorld;
    }

    [SerializeField] private PathPreset[] _presets;
    [SerializeField] private TMP_Dropdown _presetsDropdown;

    [Header("Path Settings")]
    [SerializeField] private PathPreset _pathPreset;

    [Header("Path Settings")]
    [SerializeField] private PathSettings _pathSettings;

    [Header("Generation Data")]
    [SerializeField] private GenerationData _generationData;
    
    [Header("Instances")]
    [SerializeField] private PathGenerator _pathGenerator;
    [SerializeField] private PathDisplay _pathDisplay;
    [SerializeField] private WaypointsExtractor _waypointsExtractor;

    public static EventHandler OnPathGenerationStarted;
    public static EventHandler<OnPathGeneratedEventArgs> OnPathGenerationEnded;

    private void Awake()
    {
        _presetsDropdown.ClearOptions();

        List<TMP_Dropdown.OptionData> options = new();
        foreach (var preset in _presets)
        {
            options.Add(new(preset.name));
        }

        _presetsDropdown.AddOptions(options);
        _presetsDropdown.RefreshShownValue();
    }

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
        _pathDisplay.SetGenerationData(_generationData);
        _pathGenerator.SetPathSettings(_pathSettings);
        _pathDisplay.SetPathSettings(_pathSettings);

        MazeGenerator generator = _pathGenerator.GeneratePath();

        _pathDisplay.SetEntranceDirection(GetAccessPointDir(_generationData.StartPoint));
        _pathDisplay.SetExitDirection(-GetAccessPointDir(_generationData.EndPoint));
        
        await _pathDisplay.GenerateTilemap(generator);
        
        _waypointsExtractor.CacheRules();
        _waypointsExtractor.SetStartPoint(_pathDisplay.GetStartPointWorld());
        _waypointsExtractor.ExtractWaypoints();
        
        OnPathGenerationEnded?.Invoke(this, new OnPathGeneratedEventArgs
        {
            StartPointWorld = _pathDisplay.GetStartPointWorld(),
            EndPointWorld = _pathDisplay.GetEndPointWorld()
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

    public void SetPresetByIndex(int index)
    {
        _pathPreset = _presets[index];
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
        _pathDisplay.SetPathSettings(_pathSettings);
        _pathGenerator.SetGenerationData(_generationData);
        _pathDisplay.SetGenerationData(_generationData);
    }
}
