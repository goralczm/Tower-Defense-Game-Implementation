using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _speed = 1f;
    
    private int _currentWaypointIndex;
    private bool _isStopped;

    public float PathTraveled => GetDistanceOnPath() /*/ WaypointsParent.Instance.Length*/;
    public int DifficultyLevel => 1;
    public PathColor PathColor => PathColor.Red;

    public float GetDistanceOnPath()
    {
        return 1;

        /*List<Vector2> waypointsBehind = WaypointsParent.Instance.Waypoints.Take(_currentWaypointIndex).ToList();

        waypointsBehind.Add(transform.position);

        return Helpers.CalculatePathLength(waypointsBehind);*/
    }
    
    public void ResetCache()
    {
        _currentWaypointIndex = 0;

        //PathGenerationOrchestrator.OnPathGenerationStarted += Stop;
        //PathGenerationOrchestrator.OnPathGenerationEnded += FindNearestPoint;
    }

    private void OnDisable()
    {
        //PathGenerationOrchestrator.OnPathGenerationStarted -= Stop;
        //PathGenerationOrchestrator.OnPathGenerationEnded -= FindNearestPoint;
    }

    private void Stop(object sender, EventArgs args)
    {
        _isStopped = true;
    }
    
    private void FindNearestPoint(object sender, PathOrchestrator.OnPathGeneratedEventArgs args)
    {
        //_currentWaypointIndex = WaypointsParent.Instance.GetIndexOfNearestWaypoint(transform.position);
        _isStopped = false;
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        if (_isStopped) return;
        
        /*transform.position = Vector2.MoveTowards(transform.position, WaypointsParent.Instance.Waypoints[_currentWaypointIndex], _speed * Time.deltaTime);
        if ((Vector2)transform.position == WaypointsParent.Instance.Waypoints[_currentWaypointIndex])
        {
            if (_currentWaypointIndex >= WaypointsParent.Instance.Waypoints.Count - 1)
            {
                gameObject.SetActive(false);
                return;
            }

            _currentWaypointIndex++;
        }*/
    }
}
