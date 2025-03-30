using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PathsParent))]
public class PathsParentEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Randomize"))
            ((PathsParent)target).Randomize();
    }
}
