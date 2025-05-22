using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PathSettings))]
public class PathSettingsEditor : Editor
{
    private PathSettings _pathSettings;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        _pathSettings = (PathSettings)target;
        
        if (GUILayout.Button("Set Random Steps"))
            _pathSettings.SetRandomSteps();
    }
}
