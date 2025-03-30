using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _speed = 1f;
    [SerializeField] private float _stoppingDistance = 0.1f;
    [SerializeField] private float _rotationSpeed = 5f;
    
    private int _currentWaypointIndex;
    private Vector2 _currentWaypoint;
    private Vector2 _direction;
    
    private void Start()
    {
        _currentWaypoint = WaypointsParent.Instance.Waypoints[_currentWaypointIndex];
        _direction = (_currentWaypoint - (Vector2)transform.position).normalized;
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        if (Vector2.Distance(transform.position, _currentWaypoint) <= _stoppingDistance)
        {
            _currentWaypointIndex++;
            if (_currentWaypointIndex >= WaypointsParent.Instance.Waypoints.Count)
            {
                Destroy(gameObject);
                return;
            }
            _currentWaypoint = WaypointsParent.Instance.Waypoints[_currentWaypointIndex];
            _direction = (_currentWaypoint - (Vector2)transform.position).normalized;
        }
        
        transform.position = Vector2.MoveTowards(transform.position, _currentWaypoint, _speed * Time.deltaTime);
        Rotate();
    }

    private void Rotate()
    {
        Vector2 direction = _currentWaypoint - (Vector2)transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, _rotationSpeed * Time.deltaTime);
    }
}
