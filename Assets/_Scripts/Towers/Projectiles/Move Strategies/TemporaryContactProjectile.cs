using System;
using UnityEngine;

namespace Towers.Projectiles
{
    public class TemporaryContactProjectile : IProjectileMovement
    {
        public float DestroyDelay = 10f;

        private ProjectileBehaviour _projectile;

        public event Action<Transform> OnTransformCollision;

        public bool DestroyIfInvalidTarget => false;

        public void Init(ProjectileBehaviour projectile)
        {
            _projectile = projectile;
            _projectile.Invoke("DestroyProjectile", DestroyDelay);
        }

        private void DestroyProjectile()
        {
            _projectile.DestroyProjectile();
        }

        public void Move(Vector2 target)
        {
            _projectile.transform.position = Vector2.Lerp(_projectile.transform.position, target, Time.deltaTime * 10f);

            var targets = Targeting.Targeting.GetTargetsInRange(_projectile.transform.position, _projectile.ContactRadius, _projectile.CanDamageAlignments);
            foreach (var t in targets)
            {
                OnTransformCollision?.Invoke(t.Transform);
                _projectile.DestroyProjectile();
                return;
            }
        }

        public IProjectileMovement Clone()
        {
            return new TemporaryContactProjectile()
            {
                DestroyDelay = DestroyDelay,
            };
        }
    }
}
