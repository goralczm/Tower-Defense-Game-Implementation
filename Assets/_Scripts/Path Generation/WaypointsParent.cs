using System.Collections.Generic;
using UnityEngine;

public class WaypointsParent : MonoBehaviour
{
    public static WaypointsParent Instance;

    [Header("Debug")]
    [SerializeField] private bool _debug;
    [SerializeField] private float _waypointsSize = .1f;

    private void Awake()
    {
        Instance = this;
    }

    [SerializeField] private PathsParent _pathsParent;

    public List<Vector2> Waypoints => GetWaypoints();
    private List<Vector2> _waypoints = new();

    private List<Vector2> GetWaypoints()
    {
       return _pathsParent.GetWaypoints();
    }

    private void OnDrawGizmos()
    {
        if (!_debug) return;

        Gizmos.color = Color.red;

        foreach (var waypoint in Waypoints)
        {
            Gizmos.DrawWireSphere(waypoint, _waypointsSize);
        }
    }
}
