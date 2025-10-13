using System.Collections.Generic;
using UnityEngine;

namespace Paths
{
    [System.Serializable]
    public class Path : MonoBehaviour
    {
        private List<Vector2> _waypoints = new();
        private float _pathLength;
        private Dictionary<int, float> _distanceByWaypoint = new();

        public List<Vector2> Waypoints => _waypoints;
        public float Length => _pathLength;

        public void SetWaypoints(List<Vector2> waypoints)
        {
            _waypoints = waypoints;

            _distanceByWaypoint.Clear();
            _distanceByWaypoint.Add(0, 0);
            for (int i = 1; i < waypoints.Count; i++)
            {
                float currentLength = _distanceByWaypoint[i - 1];
                currentLength += Vector2.Distance(waypoints[i - 1], waypoints[i]);
                _distanceByWaypoint.Add(i, currentLength);
            }

            _pathLength = _distanceByWaypoint[_waypoints.Count - 1];
        }

        public float GetDistanceOnPath(Vector2 position, int lastWaypointIndex)
        {
            if (lastWaypointIndex <= 0) return 0;

            return _distanceByWaypoint[lastWaypointIndex] + Vector2.Distance(position, Waypoints[lastWaypointIndex]);
        }

        public int GetIndexOfNearestWaypoint(Vector2 position)
        {
            List<Vector2> cp = new List<Vector2>(Waypoints);
            cp
                .Sort((w1, w2) => Vector2.Distance(position, w1)
                .CompareTo(Vector2.Distance(position, w2)));

            return Waypoints.IndexOf(cp[0]);
        }
    }
}
