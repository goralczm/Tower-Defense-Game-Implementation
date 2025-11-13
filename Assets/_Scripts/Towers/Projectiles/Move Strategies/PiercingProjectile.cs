using Attributes;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Towers.Projectiles
{
    public class PiercingProjectile : IProjectileMovement
    {
        private ProjectileBehaviour _projectile;
        private HashSet<Collider2D> _damagedTargets = new();
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

            Collider2D[] hits = Physics2D.OverlapCircleAll(_projectile.transform.position, .1f);
            foreach (var hit in hits)
            {
                if (_damagedTargets.Contains(hit))
                    continue;

                OnTransformCollision?.Invoke(hit.transform);
                _damagedTargets.Add(hit);
            }

            if (_projectile.IsNearTarget(_targetPosition.Value))
                _projectile.DestroyProjectile();
        }

        public IProjectileMovement Clone() => new PiercingProjectile();
    }
}
