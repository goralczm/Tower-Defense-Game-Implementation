using _Scripts.Map_Generator.Core.Map;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
    private MapGenerator _mapGenerator;
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        _mapGenerator = (MapGenerator)target;

        if (GUILayout.Button("Regenerate Path"))
            _mapGenerator.GenerateMap();

        if (GUILayout.Button("Generate With New Seed"))
        {
            _mapGenerator.SetSeed(Randomizer.GetRandomSeed());
            _mapGenerator.GenerateMap();
        }
    }
}
