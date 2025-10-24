using System;
using System.Collections.Generic;
using UnityEngine;

namespace Towers.Projectiles
{
    public class PermanentContactProjectile : IProjectileMoveStrategy
    {
        private ProjectileBehaviour _projectile;
        private Dictionary<Transform, float> _targetCooldowns = new();

        private const float DAMAGE_COOLDOWN = .25f;

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
                if (_targetCooldowns.TryGetValue(t.Transform, out var cooldown) && Time.time < cooldown)
                    continue;

                OnTransformCollision?.Invoke(t.Transform);
                _targetCooldowns[t.Transform] = Time.time + DAMAGE_COOLDOWN;
            }
        }

        public IProjectileMoveStrategy Clone() => new PermanentContactProjectile();
    }
}
