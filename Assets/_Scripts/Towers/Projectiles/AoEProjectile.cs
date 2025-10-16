using Towers.Projectiles;
using UnityEngine;

namespace Towers
{
    public class AoEProjectile : ProjectileBase
    {
        public override void Tick()
        {
            base.Tick();

            if (!_target) return;

            if (!_target.gameObject.activeSelf)
            {
                DestroyProjectile();
                return;
            }

            _targetPosition = _target.position;

            Move();
        }

        private void Move()
        {
            if (IsNearTarget(_targetPosition))
            {
                var hits = Physics2D.OverlapCircleAll(transform.position, _attributes.GetAttribute(Attributes.ProjectileAttributes.Range));
                foreach (var hit in hits)
                    TryDamageTarget(hit.transform, _canDamageAlignments);

                DestroyProjectile();
                return;
            }

            transform.position = Vector2.MoveTowards(transform.position, _targetPosition, Time.deltaTime * _attributes.GetAttribute(Attributes.ProjectileAttributes.Speed));
        }
    }
}
