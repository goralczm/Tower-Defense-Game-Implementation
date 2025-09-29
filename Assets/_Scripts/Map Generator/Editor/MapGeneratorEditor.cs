using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[CustomEditor(typeof(MapGenerator.Core.Map.MapGenerator))]
public class MapGeneratorEditor : Editor
{
    private MapGenerator.Core.Map.MapGenerator _mapGenerator;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        _mapGenerator = (MapGenerator.Core.Map.MapGenerator)target;

        if (GUILayout.Button("Regenerate Path"))
            _mapGenerator.GenerateMapByContext();

        if (GUILayout.Button("Generate With New Seed"))
        {
            _mapGenerator.SetSeed(Randomizer.GetRandomSeed());
            _mapGenerator.GenerateRandomMap();
        }

        Tilemap mapTilemap = _mapGenerator.GetTilemap();
        if (mapTilemap.transform.position.z != 0)
        {
            EditorGUILayout.HelpBox("Error: Tilemap Z position must be 0 in order to work properly!", MessageType.Error);
            if (GUILayout.Button("Reset Tilemap Z Position"))
                mapTilemap.transform.position =
                    new Vector3(mapTilemap.transform.position.x, mapTilemap.transform.position.y, 0);
        }
    }
}
