using System;
using UnityEngine;

namespace Towers.Projectiles
{
    public class PointProjectile : IProjectileMoveStrategy
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
            _projectile.transform.position = Vector2.MoveTowards(_projectile.transform.position, target, Time.deltaTime * _projectile.Attributes.GetAttribute(Attributes.ProjectileAttributes.Speed));

            if (_projectile.IsNearTarget(target))
            {
                var targets = Targeting.Targeting.GetTargetsInRange(_projectile.transform.position, .2f, _projectile.CanDamageAlignments);
                if (targets.Count > 0)
                    OnTransformCollision?.Invoke(targets[0].Transform);

                _projectile.DestroyProjectile();
            }
        }

        public IProjectileMoveStrategy Clone() => new PointProjectile();
    }
}
