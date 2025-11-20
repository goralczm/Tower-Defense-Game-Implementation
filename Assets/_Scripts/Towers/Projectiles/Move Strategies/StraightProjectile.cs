using Attributes;
using System;
using UnityEngine;

namespace Towers.Projectiles
{
    public class StraightProjectile : IProjectileMovement
    {
        private ProjectileBehaviour _projectile;
        private Vector2? _targetPosition = null;

        public event Action<Transform> OnTransformCollision;

        public bool DestroyIfInvalidTarget => false;

        public void Init(ProjectileBehaviour projectile)
        {
            _projectile = projectile;
        }

        public void Move(Vector2 target)
        {
            if (!_targetPosition.HasValue)
                _targetPosition = (Vector2)_projectile.transform.position + (target - (Vector2)_projectile.transform.position).normalized * _projectile.Attributes.GetAttribute(ProjectileAttributes.Range);

            _projectile.transform.position = Vector2.MoveTowards(_projectile.transform.position, _targetPosition.Value, Time.deltaTime * _projectile.Attributes.GetAttribute(Attributes.ProjectileAttributes.Speed));

            var targets = Targeting.Targeting.GetTargetsInRange(_projectile.transform.position, .05f, _projectile.CanDamageAlignments);
            foreach (var t in targets)
            {
                OnTransformCollision?.Invoke(t.Transform);
                _projectile.DestroyProjectile();
                return;
            }

            if (_projectile.IsNearTarget(_targetPosition.Value))
                _projectile.DestroyProjectile();
        }

        public IProjectileMovement Clone() => new StraightProjectile();
    }
}
