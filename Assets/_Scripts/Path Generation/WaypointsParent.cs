using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class WaypointsParent : Singleton<WaypointsParent>
{
    [Header("Debug")]
    [SerializeField] private bool _debug;
    [SerializeField] private float _waypointsSize = .1f;

    [SerializeField] private PathDisplay _pathDisplay;

    public List<Vector2> Waypoints = new();

    protected override void Awake()
    {
        base.Awake();

        PathDisplay.OnPathGenerated += CacheWaypoints;
    }

    private void OnDisable()
    {
        PathDisplay.OnPathGenerated -= CacheWaypoints;
    }

    private void CacheWaypoints()
    {
        Waypoints.Clear();
        Waypoints = _pathDisplay.GetWaypoints();
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
