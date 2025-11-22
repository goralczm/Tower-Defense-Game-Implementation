using Attributes;
using Core;
using Paths;
using System;
using System.Collections;
using UnityEngine;
using Utilities;

namespace Enemies
{
    public class EnemyBehaviour : MonoBehaviour, ITargetable, IDamageable
    {
        [Header("References")]
        [SerializeField] private SpriteRenderer[] _rends;

        private Path _path;
        private Attributes<EnemyAttributes> _attributes;
        private EnemyData _enemyData;
        private int _currentWaypointIndex;
        private bool _isStopped;

        public event Action<DamageData> OnDamaged;

        public static event Action<EnemyBehaviour, DeathReason> OnEnemyDied;
        public static event Action<EnemyData, int, Vector2> SpawnEnemyRequest;
        public static event Action<EnemyData> OnEnemyReachedPathEnd;

        public EnemyData EnemyData => _enemyData;
        public Attributes<EnemyAttributes> Attributes => _attributes;
        public Alignment Alignment => Alignment.Hostile;
        public Transform Transform => transform;
        public int Strength => _enemyData.DangerLevel;
        public int TargetingPriority => 1;

        public float GetDistance(Vector2 position) => GetDistanceOnPath() / _path.Length;

        private void Start()
        {
            IDamageable.RecordDamageRequest?.Invoke(this);
        }

        public void Setup(EnemyData enemyData, Path path, int nextWaypointIndex)
        {
            _path = path;
            _enemyData = enemyData;
            _currentWaypointIndex = nextWaypointIndex;
            _attributes = new(new(), enemyData.BaseAttributes);
            foreach (var rend in _rends)
                rend.color = _enemyData.Color;
        }

        public float GetDistanceOnPath()
        {
            return _path.GetDistanceOnPath(transform.position, _currentWaypointIndex - 1);
        }

        private void Update()
        {
            _attributes.Mediator.Update(Time.deltaTime);

            transform.localScale = Vector3.one * _attributes.GetAttribute(EnemyAttributes.Size, 1f);

            MoveTowardsWaypoint();
        }

        private void MoveTowardsWaypoint()
        {
            if (_isStopped) return;

            if (_path.Waypoints.Count == 0)
            {
                Die(DeathReason.Self);
                return;
            }

            var position = (Vector2)transform.position;
            var target = _path.Waypoints[_currentWaypointIndex];

            Quaternion targetRotation = Helpers.RotateTowards(position, target, -90f);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 15f);

            position = Vector2.MoveTowards(position, target, _attributes.GetAttribute(EnemyAttributes.Speed) * Time.deltaTime);
            transform.position = position;

            if ((position - target).sqrMagnitude < 0.000001f)
            {
                if (_currentWaypointIndex >= _path.Waypoints.Count - 1)
                {
                    OnEnemyReachedPathEnd?.Invoke(_enemyData);
                    Die(DeathReason.Self);
                    return;
                }

                _currentWaypointIndex++;
            }
        }

        public void TakeDamage(float damage, DamageType[] types, string source)
        {
            damage = CalculateDamage(damage, types);

            if (damage == 0) return;

            OnDamaged?.Invoke(new(
                Mathf.Min(_attributes.GetAttribute(EnemyAttributes.Health), damage),
                source,
                _enemyData.name,
                types,
                transform.position));

            var modifier = new BasicAttributeModifier<EnemyAttributes>(EnemyAttributes.Health, 0f, v => v - damage);
            _attributes.Mediator.AddModifier(modifier);

            if (_attributes.GetAttribute(EnemyAttributes.Health) <= 0f)
            {
                SpawnChildren(transform.position, _currentWaypointIndex - 1, .2f, _enemyData.Children.Count);
                Die(DeathReason.External);
            }
            /*else if (_rends.color != Color.red)
                StartCoroutine(HitEffect());*/
        }

        private float CalculateDamage(float damage, DamageType[] types)
        {
            if (types.Length != 0)
            {
                int validResistancesCount = 0;
                int validVulnerabilitiesCount = 0;

                foreach (var damageType in types)
                {
                    foreach (var resistance in _enemyData.Resistances)
                    {
                        if (damageType == resistance)
                            validResistancesCount++;
                    }

                    foreach (var vulnerability in _enemyData.Vulnerabilities)
                    {
                        if (damageType == vulnerability)
                            validVulnerabilitiesCount++;
                    }
                }

                damage *= 1 - validResistancesCount / types.Length;
                damage *= 1 + validVulnerabilitiesCount / types.Length;
            }

            return damage;
        }

        public void Die(DeathReason reason)
        {
            OnEnemyDied?.Invoke(this, reason);
            StopAllCoroutines();
            //_rends.color = Color.white;
            gameObject.SetActive(false);
        }

        private void SpawnChildren(Vector2 pointA, int lastPointIndex, float spacing, int points)
        {
            if (points <= 0 || lastPointIndex < 0 || _enemyData.Children.Count == 0)
                return;

            float distance = Vector2.Distance(pointA, _path.Waypoints[lastPointIndex]);
            int pointsCapacity = (int)(distance / spacing);
            int range = Mathf.Min(pointsCapacity, points);
            for (int i = 0; i < range; i++)
            {
                Vector2 newChildPos = Vector2.Lerp(_path.Waypoints[lastPointIndex], pointA, (distance - spacing * (i + 1)) / distance);
                SpawnEnemyRequest?.Invoke(_enemyData.Children[i], lastPointIndex + 1, newChildPos);
            }

            SpawnChildren(_path.Waypoints[lastPointIndex], lastPointIndex - 1, spacing, points - range);
        }

        IEnumerator HitEffect()
        {
            //_rends.color = Color.red;
            yield return new WaitForSeconds(.1f);
            //_rends.color = Color.white;
        }
    }
}
