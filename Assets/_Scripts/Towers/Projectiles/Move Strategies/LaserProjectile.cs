using System;
using Towers.Projectiles;
using UnityEngine;

namespace Towers
{
    public class LaserProjectile : IProjectileMovement
    {
        private ProjectileBehaviour _projectile;
        private LineRenderer _line;

        public event Action<Transform> OnTransformCollision;

        public bool DestroyIfInvalidTarget => false;

        public void Init(ProjectileBehaviour projectile)
        {
            _projectile = projectile;
            _line = _projectile.gameObject.AddComponent<LineRenderer>();
            _line.positionCount = 2;
            _line.SetPosition(0, _projectile.transform.position);
        }

        public void Move(Vector2 target)
        {
            _line.SetPosition(1, target);

            if (_projectile.Target)
                OnTransformCollision?.Invoke(_projectile.Target);
        }

        public IProjectileMovement Clone() => new LaserProjectile();
    }
}
