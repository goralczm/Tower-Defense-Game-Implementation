using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PathDisplay))]
public class PathDisplayEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Randomize"))
            ((PathDisplay)target).Randomize();
    }
}
