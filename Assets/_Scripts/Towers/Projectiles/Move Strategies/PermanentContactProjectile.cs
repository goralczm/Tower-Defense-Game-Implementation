using System;
using System.Collections.Generic;
using UnityEngine;

namespace Towers.Projectiles
{
    public class PermanentContactProjectile : IProjectileMovement
    {
        private ProjectileBehaviour _projectile;

        public event Action<Transform> OnTransformCollision;

        public bool DestroyIfInvalidTarget => false;

        public void Init(ProjectileBehaviour projectile)
        {
            _projectile = projectile;
        }

        public void Move(Vector2 target)
        {
            _projectile.transform.position = target;

            var targets = Targeting.Targeting.GetTargetsInRange(_projectile.transform.position, _projectile.transform.localScale.x, _projectile.CanDamageAlignments);
            foreach (var t in targets)
            {
                OnTransformCollision?.Invoke(t.Transform);
            }
        }

        public IProjectileMovement Clone() => new PermanentContactProjectile();
    }
}
