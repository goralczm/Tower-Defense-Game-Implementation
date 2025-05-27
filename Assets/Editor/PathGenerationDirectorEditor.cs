using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PathGenerationDirector))]
public class PathGenerationDirectorEditor : Editor
{
    private PathGenerationDirector _pathGenerationDirector;
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        _pathGenerationDirector = (PathGenerationDirector)target;

        if (GUILayout.Button("Fill Tilemap"))
            FindFirstObjectByType<PathDisplay>().FillEntireMap();

        if (GUILayout.Button("Clear Tilemap"))
            FindFirstObjectByType<PathDisplay>().ClearAllTiles();

        if (GUILayout.Button("Regenerate Path"))
            _pathGenerationDirector.RegeneratePath();
        
        if (GUILayout.Button("Generate With New Seed"))
            _pathGenerationDirector.GenerateWithNewSeed();
    }
}
