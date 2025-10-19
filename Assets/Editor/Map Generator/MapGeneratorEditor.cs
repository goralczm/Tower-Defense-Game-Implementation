using MapGenerator.Demo;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapGenerator.Demo.MapGenerator))]
public class MapGeneratorEditor : Editor
{
    private MapGenerator.Demo.MapGenerator _mapGenerator;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        _mapGenerator = (MapGenerator.Demo.MapGenerator)target;

        if (GUILayout.Button("Prepare Builder"))
            _mapGenerator.PrepareBuilder();

        if (GUILayout.Button("Continue Builder"))
            _mapGenerator.ContinueBuilderAsync();

        if (GUILayout.Button("Randomize Config"))
            _mapGenerator.RandomizeConfig();

        if (GUILayout.Button("Generate Map"))
            _mapGenerator.GenerateMapAsync();
    }
}
