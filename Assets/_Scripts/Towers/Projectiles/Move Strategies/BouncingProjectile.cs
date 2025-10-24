using Attributes;
using System;
using UnityEngine;

namespace Towers.Projectiles
{
    public class BouncingProjectile : IProjectileMoveStrategy
    {
        private ProjectileBehaviour _projectile;
        private Transform _target;
        private int _bounces;

        public event Action<Transform> OnTransformCollision;
        public bool DestroyIfInvalidTarget => false;

        public void Init(ProjectileBehaviour projectile)
        {
            _projectile = projectile;
            _bounces = (int)_projectile.Attributes.GetAttribute(ProjectileAttributes.Bounces);
            _target = _projectile.Target;
        }

        public void Move(Vector2 target)
        {
            if (_target == null || !_target.gameObject.activeSelf)
                FindNewTarget();

            _projectile.transform.position = Vector2.MoveTowards(_projectile.transform.position, _target.position, Time.deltaTime * _projectile.Attributes.GetAttribute(ProjectileAttributes.Speed));

            if (_projectile.IsNearTarget(_target.position))
            {
                OnTransformCollision?.Invoke(_target);
                FindNewTarget();
                _bounces--;
            }

            if (_bounces < 0)
                _projectile.DestroyProjectile();
        }

        private void FindNewTarget()
        {
            var targets = Targeting.Targeting.GetTargetsInRange(_projectile.transform.position, 1f, _projectile.CanDamageAlignments, _target);
            if (targets.Count > 0)
                _target = targets[0].Transform;
            else
                _projectile.DestroyProjectile();
        }

        public IProjectileMoveStrategy Clone() => new BouncingProjectile();
    }
}
