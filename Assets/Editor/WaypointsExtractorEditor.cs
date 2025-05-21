using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WaypointsExtractor))]
public class WaypointsExtractorEditor : Editor
{
    private WaypointsExtractor _waypointsExtractor;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        _waypointsExtractor = (WaypointsExtractor)target;

        if (GUILayout.Button("Cache Rules"))
            _waypointsExtractor.CacheRules();

        if (GUILayout.Button("Clear Waypoints"))
            _waypointsExtractor.ClearWaypoints();

        if (GUILayout.Button("Extract Waypoints"))
            _waypointsExtractor.ExtractWaypoints();
    }
}
