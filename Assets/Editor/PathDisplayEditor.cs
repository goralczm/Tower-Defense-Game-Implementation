using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PathDisplay))]
public class PathDisplayEditor : Editor
{
    private PathDisplay _pathDisplay;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        _pathDisplay = (PathDisplay)target;

        if (GUILayout.Button("Regenerate"))
            _pathDisplay.GenerateTilemap();

        if (GUILayout.Button("Generate New"))
        {
            _pathDisplay.SetRandomSeed();
            _pathDisplay.GenerateTilemap();
        }

        if (GUILayout.Button("Randomize"))
            _pathDisplay.Randomize();
    }
}
