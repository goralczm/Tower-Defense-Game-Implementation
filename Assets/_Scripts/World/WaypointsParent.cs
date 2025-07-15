using System.Collections.Generic;
using _Scripts.Utilities;
using UnityEngine;

[System.Serializable]
public class WaypointsParent : Singleton<WaypointsParent>
{
    [Header("Debug")]
    [SerializeField] private bool _debug;
    [SerializeField] private Color _waypointColor = Color.red;
    [SerializeField] private float _waypointsSize = .1f;

    private List<Vector2> _waypoints = new();
    private float _length;

    public List<Vector2> Waypoints => _waypoints;
    public float Length => _length;

    public void CacheWaypoints(List<Vector2> waypoints)
    {
        _waypoints = waypoints;
        _length = Helpers.CalculatePathLength(Waypoints);
    }

    public int GetIndexOfNearestWaypoint(Vector2 position)
    {
        List<Vector2> cp = new List<Vector2>(Waypoints);
        cp.Sort((w1, w2) => Vector2.Distance(position, w1).CompareTo(Vector2.Distance(position, w2)));

        return Waypoints.IndexOf(cp[0]);
    }

    private void OnDrawGizmos()
    {
        if (!_debug) return;

        Gizmos.color = _waypointColor;

        foreach (var waypoint in Waypoints)
            Gizmos.DrawWireSphere(waypoint, _waypointsSize);
    }
}
