using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _speed = 1f;
    
    private int _currentWaypointIndex;
    private Vector2 _currentWaypoint;

    public void ResetCache()
    {
        _currentWaypointIndex = 0;
        _currentWaypoint = WaypointsParent.Instance.Waypoints[_currentWaypointIndex];

        PathDisplay.OnPathGenerated += FindNearestPoint;
    }

    private void OnDisable()
    {
        PathDisplay.OnPathGenerated -= FindNearestPoint;
    }

    private void FindNearestPoint()
    {
        _currentWaypointIndex = WaypointsParent.Instance.GetIndexOfNearestWaypoint(transform.position);
        _currentWaypoint = WaypointsParent.Instance.Waypoints[_currentWaypointIndex];
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        transform.position = Vector2.MoveTowards(transform.position, WaypointsParent.Instance.Waypoints[_currentWaypointIndex], _speed * Time.deltaTime);
        if ((Vector2)transform.position == WaypointsParent.Instance.Waypoints[_currentWaypointIndex])
        {
            if (_currentWaypointIndex >= WaypointsParent.Instance.Waypoints.Count - 1)
            {
                gameObject.SetActive(false);
                return;
            }

            _currentWaypointIndex++;
        }
    }
}
