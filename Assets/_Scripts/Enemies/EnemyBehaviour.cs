using Attributes;
using Core;
using Paths;
using System;
using UnityEngine;

namespace Enemies
{
    public class EnemyBehaviour : MonoBehaviour, ITargetable, IDamageable
    {
        private Path _path;
        private Attributes<EnemyAttributes> _attributes;
        private EnemyData _enemyData;
        private int _currentWaypointIndex;
        private bool _isStopped;

        public static event Action<EnemyBehaviour> OnEnemyDied;

        public Attributes<EnemyAttributes> Attributes => _attributes;
        public Alignment Alignment => Alignment.Hostile;
        public Transform Transform => transform;
        public int Strength => _enemyData.DangerLevel;
        public int Priority => 1;

        public float GetDistance(Vector2 position) => GetDistanceOnPath() / _path.Length;

        public void Setup(EnemyData enemyData, Path path, int nextWaypointIndex)
        {
            _path = path;
            _enemyData = enemyData;
            _currentWaypointIndex = nextWaypointIndex;
            _attributes = new(new(), enemyData.BaseAttributes);
        }

        public float GetDistanceOnPath()
        {
            return _path.GetDistanceOnPath(transform.position, _currentWaypointIndex - 1);
        }

        private void Update()
        {
            _attributes.Mediator.Update(Time.deltaTime);

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

            var position = (Vector2)transform.position;
            var target = _path.Waypoints[_currentWaypointIndex];

            position = Vector2.MoveTowards(position, target, _attributes.GetAttribute(EnemyAttributes.Speed) * Time.deltaTime);
            transform.position = position;

            if ((position - target).sqrMagnitude < 0.000001f)
            {
                if (_currentWaypointIndex >= _path.Waypoints.Count - 1)
                {
                    Die();
                    return;
                }

                _currentWaypointIndex++;
            }
        }

        public void TakeDamage(float damage)
        {
            var modifier = new BasicAttributeModifier<EnemyAttributes>(EnemyAttributes.Health, 0f, v => v - damage);

            _attributes.Mediator.AddModifier(modifier);

            if (_attributes.GetAttribute(EnemyAttributes.Health) <= 0f)
                Die();
        }

        public void Die()
        {
            gameObject.SetActive(false);
            OnEnemyDied?.Invoke(this);
        }

        private void OnMouseDown()
        {
            var modifier = new BasicAttributeModifier<EnemyAttributes>(EnemyAttributes.Speed, 2f, v => v + 2);

            _attributes.Mediator.AddModifier(modifier);
        }
    }
}
