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
        
        if (_pathSettings.ExperimentalRandomizeSeedWhenGenerationDepthExceeded)
            EditorGUILayout.HelpBox("Warning: Experimental feature enabled! This may lead to endless loop if the settings are unreachable. Use with caution.", MessageType.Warning);

        if (GUILayout.Button("Set Random Steps"))
            _pathSettings.SetRandomSteps();
    }
}
