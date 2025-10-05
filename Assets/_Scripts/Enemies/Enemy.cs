using Paths;
using UnityEngine;

namespace Enemies
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private float _speed = 1f;

        private int _currentWaypointIndex;
        private bool _isStopped;

        public float PathTraveled => GetDistanceOnPath() / _path.Length;
        public int DifficultyLevel => 1;
        public PathColor PathColor => PathColor.Red;

        private Path _path;

        public void SetPath(Path path) => _path = path;

        public void Reset()
        {
            _currentWaypointIndex = 0;
        }

        public float GetDistanceOnPath()
        {
            return _path.GetDistanceOnPath(transform.position, _currentWaypointIndex - 1);
        }

        private void Update()
        {
            MoveTowardsWaypoint();
        }

        private void MoveTowardsWaypoint()
        {
            if (_isStopped) return;

            if (_path.Waypoints.Count == 0)
            {
                Die();
                return;
            }

            transform.position = Vector2.MoveTowards(transform.position, _path.Waypoints[_currentWaypointIndex], _speed * Time.deltaTime);
            if ((Vector2)transform.position == _path.Waypoints[_currentWaypointIndex])
            {
                if (_currentWaypointIndex >= _path.Waypoints.Count - 1)
                {
                    Die();
                    return;
                }

                _currentWaypointIndex++;
            }
        }

        public void Die()
        {
            gameObject.SetActive(false);

        }
    }
}
