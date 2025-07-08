using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PathGenerationOrchestrator))]
public class PathGenerationDirectorEditor : Editor
{
    private PathGenerationOrchestrator _pathGenerationOrchestrator;
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        _pathGenerationOrchestrator = (PathGenerationOrchestrator)target;

        if (GUILayout.Button("Fill Tilemap"))
            FindFirstObjectByType<PathRenderer>().FillEntireMap();

        if (GUILayout.Button("Clear Tilemap"))
            FindFirstObjectByType<PathRenderer>().ClearAllTiles();

        if (GUILayout.Button("Regenerate Path"))
            _pathGenerationOrchestrator.RegeneratePath();
        
        if (GUILayout.Button("Generate With New Seed"))
            _pathGenerationOrchestrator.GenerateWithNewSeed();
    }
}
