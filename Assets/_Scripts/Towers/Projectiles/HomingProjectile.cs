using Core;
using UnityEngine;

namespace Towers
{
    public class HomingProjectile : ProjectileBase
    {
        [SerializeField] private float _speed = 3f;

        private const float STOPPING_DISTANCE = 0.05f;

        private void Update()
        {
            if (!_target) return;

            if (!_target.gameObject.activeSelf)
            {
                DestroyProjectile();
                return;
            }

            Move();
        }

        private void Move()
        {
            Vector3 position = transform.position;
            Vector3 target = _target.position;

            bool isNearTarget = (position - target).sqrMagnitude < STOPPING_DISTANCE;
            if (isNearTarget)
            {
                TryDamageTarget(_target);
                DestroyProjectile();
                return;
            }

            transform.position = Vector3.MoveTowards(position, target, Time.deltaTime * _speed);
        }

        private void TryDamageTarget(Transform target)
        {
            if (target.TryGetComponent(out IDamageable damageable))
                damageable.TakeDamage(_damage);
        }

        public void DestroyProjectile()
        {
            Destroy(gameObject);
        }
    }
}
