using Attributes;
using System;
using Towers.Projectiles;
using UnityEngine;

namespace Towers
{
    public class LaserProjectile : IProjectileMovement
    {
        private ProjectileBehaviour _projectile;
        private LineRenderer _line;

        private float _tickTimer;

        public event Action<Transform> OnTransformCollision;

        public bool DestroyIfInvalidTarget => false;

        public void Init(ProjectileBehaviour projectile)
        {
            _projectile = projectile;
            _line = _projectile.gameObject.GetComponent<LineRenderer>() ?? _projectile.gameObject.AddComponent<LineRenderer>();
            _line.positionCount = 2;
            _line.SetPosition(0, _projectile.transform.position);
            _line.startColor = _projectile.ProjectileData.Color;
            _line.endColor = _projectile.ProjectileData.Color;
        }

        public void Move(Vector2 target)
        {
            _line.SetPosition(1, target);
            _line.widthMultiplier = _projectile.Attributes.GetAttribute(ProjectileAttributes.Size, 1f);

            if (Time.time < _tickTimer) return;

            if (_projectile.Target)
                OnTransformCollision?.Invoke(_projectile.Target);

            _tickTimer = Time.time + _projectile.Attributes.GetAttribute(ProjectileAttributes.TickRate, .1f);
        }

        public IProjectileMovement Clone() => new LaserProjectile();
    }
}
