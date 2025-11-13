using Attributes;
using System;
using UnityEngine;

namespace Towers.Projectiles
{
    public class HomingProjectile : IProjectileMovement
    {
        private ProjectileBehaviour _projectile;

        public event Action<Transform> OnTransformCollision;

        public bool DestroyIfInvalidTarget => true;

        public void Init(ProjectileBehaviour projectile)
        {
            _projectile = projectile;
        }

        public void Move(Vector2 target)
        {
            _projectile.transform.position = Vector2.MoveTowards(_projectile.transform.position, target, Time.deltaTime * _projectile.Attributes.GetAttribute(ProjectileAttributes.Speed));

            if (_projectile.IsNearTarget(target))
            {
                OnTransformCollision?.Invoke(_projectile.Target);
                _projectile.DestroyProjectile();
            }
        }

        public IProjectileMovement Clone() => new HomingProjectile();
    }
}
