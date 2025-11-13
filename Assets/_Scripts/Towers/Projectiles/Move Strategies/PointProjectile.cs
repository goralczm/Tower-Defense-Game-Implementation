using System;
using UnityEngine;

namespace Towers.Projectiles
{
    public class PointProjectile : IProjectileMovement
    {
        private ProjectileBehaviour _projectile;
        private Vector2? _target;

        public event Action<Transform> OnTransformCollision;

        public bool DestroyIfInvalidTarget => false;

        public void Init(ProjectileBehaviour projectile)
        {
            _projectile = projectile;
        }

        public void Move(Vector2 target)
        {
            if (_target == null)
                _target = target;

            _projectile.transform.position = Vector2.MoveTowards(_projectile.transform.position, _target.Value, Time.deltaTime * _projectile.Attributes.GetAttribute(Attributes.ProjectileAttributes.Speed));

            if (_projectile.IsNearTarget(_target.Value))
            {
                var targets = Targeting.Targeting.GetTargetsInRange(_projectile.transform.position, .2f, _projectile.CanDamageAlignments);
                if (targets.Count > 0)
                    OnTransformCollision?.Invoke(targets[0].Transform);

                _projectile.DestroyProjectile();
            }
        }

        public IProjectileMovement Clone() => new PointProjectile();
    }
}
