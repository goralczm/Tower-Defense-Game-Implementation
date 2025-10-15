using Core;
using UnityEngine;

namespace Towers.Projectiles
{
    public class HomingProjectile : ProjectileBase
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
                TryDamageTarget(_target);
                DestroyProjectile();
                return;
            }

            transform.position = Vector2.MoveTowards(transform.position, _targetPosition, Time.deltaTime * _attributes.GetAttribute(Attributes.ProjectileAttributes.Speed));
        }
    }
}
