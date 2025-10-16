using System.Collections.Generic;
using UnityEngine;

namespace Towers.Projectiles
{
    public class StraightProjectile : ProjectileBase
    {
        [Header("Settings")]
        [SerializeField] private bool _isPiercing;

        private HashSet<Collider2D> _damagedTargets = new();

        public override void Init()
        {
            base.Init();
            _targetPosition = (_targetPosition - (Vector2)transform.position).normalized * _attributes.GetAttribute(Attributes.ProjectileAttributes.Range);
        }

        public override void Tick()
        {
            base.Tick();

            Move();
        }

        private void Move()
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.1f);
            foreach (var hit in hits)
            {
                if (_damagedTargets.Contains(hit))
                    continue;

                if (TryDamageTarget(hit.transform, _canDamageAlignments))
                {
                    if (!_isPiercing)
                    {
                        DestroyProjectile();
                        return;
                    }
                }

                _damagedTargets.Add(hit);
            }

            if (IsNearTarget(_targetPosition))
            {
                DestroyProjectile();
                return;
            }

            transform.position = Vector2.MoveTowards(transform.position, _targetPosition, Time.deltaTime * _attributes.GetAttribute(Attributes.ProjectileAttributes.Speed));
        }
    }
}
