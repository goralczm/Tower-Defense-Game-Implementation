using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NoiseDisplay))]
public class NoiseDisplayEditor : Editor
{
    private NoiseDisplay _noiseDisplay;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        _noiseDisplay = (NoiseDisplay)target;

        if (GUILayout.Button("Generate Map"))
            _noiseDisplay.GenerateMap();
    }
}
