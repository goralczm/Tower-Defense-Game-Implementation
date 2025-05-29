using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _speed = 1f;
    
    private int _currentWaypointIndex;
    private bool _isStopped;
    private Vector2 _currentWaypoint;

    public void ResetCache()
    {
        _currentWaypointIndex = 0;
        _currentWaypoint = WaypointsParent.Instance.Waypoints[_currentWaypointIndex];

        PathGenerationDirector.OnPathGenerationStarted += Stop;
        PathGenerationDirector.OnPathGenerationEnded += FindNearestPoint;
    }

    private void OnDisable()
    {
        PathGenerationDirector.OnPathGenerationStarted -= Stop;
        PathGenerationDirector.OnPathGenerationEnded -= FindNearestPoint;
    }

    private void Stop(object sender, EventArgs args)
    {
        _isStopped = true;
    }
    
    private void FindNearestPoint(object sender, PathGenerationDirector.OnPathGeneratedEventArgs args)
    {
        _currentWaypointIndex = WaypointsParent.Instance.GetIndexOfNearestWaypoint(transform.position);
        _currentWaypoint = WaypointsParent.Instance.Waypoints[_currentWaypointIndex];
        _isStopped = false;
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        if (_isStopped) return;
        
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
