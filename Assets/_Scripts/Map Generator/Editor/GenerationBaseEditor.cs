using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MazeGenerationSettings))]
public class GenerationBaseEditor : Editor
{
    private MazeGenerationSettings _mazeGenerationSettings;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        _mazeGenerationSettings = (MazeGenerationSettings)target;

        if (_mazeGenerationSettings.PointsStrategy == PointsStrategy.Custom)
        {
            _mazeGenerationSettings.MinStartPoint = EditorGUILayout.Vector2IntField("Min Start Point", _mazeGenerationSettings.MinStartPoint);
            _mazeGenerationSettings.MaxStartPoint = EditorGUILayout.Vector2IntField("Max Start Point", _mazeGenerationSettings.MaxStartPoint);
            _mazeGenerationSettings.MinEndPoint = EditorGUILayout.Vector2IntField("Min End Point", _mazeGenerationSettings.MinEndPoint);
            _mazeGenerationSettings.MaxEndPoint = EditorGUILayout.Vector2IntField("Max End Point", _mazeGenerationSettings.MaxEndPoint);
        }
    }
}
