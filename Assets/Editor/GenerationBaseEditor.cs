using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GenerationDataBase))]
public class GenerationBaseEditor : Editor
{
    private GenerationDataBase _generationDataBase;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        _generationDataBase = (GenerationDataBase)target;

        if (_generationDataBase.PointsStrategy == PointsStrategy.Custom)
        {
            _generationDataBase.MinStartPoint = EditorGUILayout.Vector2IntField("Min Start Point", _generationDataBase.MinStartPoint);
            _generationDataBase.MaxStartPoint = EditorGUILayout.Vector2IntField("Max Start Point", _generationDataBase.MaxStartPoint);
            _generationDataBase.MinEndPoint = EditorGUILayout.Vector2IntField("Min End Point", _generationDataBase.MinEndPoint);
            _generationDataBase.MaxEndPoint = EditorGUILayout.Vector2IntField("Max End Point", _generationDataBase.MaxEndPoint);
        }
    }
}
