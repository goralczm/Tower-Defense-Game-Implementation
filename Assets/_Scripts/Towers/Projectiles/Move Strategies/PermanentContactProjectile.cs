using Attributes;
using System;
using UnityEngine;

namespace Towers.Projectiles
{
    public class PermanentContactProjectile : IProjectileMovement
    {
        private ProjectileBehaviour _projectile;

        public event Action<Transform> OnTransformCollision;

        private float _tickTimer;

        public bool DestroyIfInvalidTarget => false;

        public void Init(ProjectileBehaviour projectile)
        {
            _projectile = projectile;
        }

        public void Move(Vector2 target)
        {
            _projectile.transform.position = Vector2.Lerp(_projectile.transform.position, target, Time.deltaTime * 10f);

            if (Time.time < _tickTimer) return;

            var targets = Targeting.Targeting.GetTargetsInRange(_projectile.transform.position, _projectile.ContactRadius, _projectile.CanDamageAlignments);
            foreach (var t in targets)
                OnTransformCollision?.Invoke(t.Transform);

            _tickTimer = Time.time + _projectile.Attributes.GetAttribute(ProjectileAttributes.TickRate, .1f);
        }

        public IProjectileMovement Clone() => new PermanentContactProjectile();
    }
}
