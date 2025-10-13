using Paths;
using System;
using UnityEngine;

namespace Enemies
{
    public class EnemyBehaviour : MonoBehaviour
    {
        private Path _path;
        private EnemyData _enemyData;
        private int _currentWaypointIndex;
        private bool _isStopped;
        private float _speed = 1f;

        public float PathTraveled => GetDistanceOnPath() / _path.Length;
        public int DangerLevel => 1;

        public static Action<EnemyBehaviour> OnEnemyDied;

        public void Setup(EnemyData enemyData, Path path, int nextWaypointIndex)
        {
            _path = path;
            _currentWaypointIndex = nextWaypointIndex;
            _enemyData = enemyData;
            _speed = enemyData.Speed;
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

        public void Die(bool notify = true)
        {
            gameObject.SetActive(false);
            if (notify)
                OnEnemyDied?.Invoke(this);
        }
    }
}
