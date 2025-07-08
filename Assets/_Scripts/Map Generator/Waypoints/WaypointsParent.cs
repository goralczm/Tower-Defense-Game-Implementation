using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using _Scripts.Utilities;
using UnityEngine;

public class WaypointsParent : Singleton<WaypointsParent>
{
    [Header("Debug")]
    [SerializeField] private bool _debug;
    [SerializeField] private float _waypointsSize = .1f;

    [SerializeField] private WaypointsExtractor _waypointExtractor;

    public List<Vector2> Waypoints = new();

    private float _length;

    public float Length => _length;

    public void CacheWaypoints(Vector2 startPoint)
    {
        _waypointExtractor.SetStartPoint(startPoint);
        _waypointExtractor.ExtractWaypoints();
        Waypoints = _waypointExtractor.GetWaypoints();
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

        Gizmos.color = Color.red;

        foreach (var waypoint in Waypoints)
            Gizmos.DrawWireSphere(waypoint, _waypointsSize);
    }
}
