using Attributes;
using UnityEngine;

namespace Towers.Projectiles
{
    public class SimpleProjectileDamage : IProjectileDamageStrategy
    {
        private ProjectileBehaviour _projectile;

        public DamageStrategyPriority Priority => DamageStrategyPriority.Damage;

        public void Init(ProjectileBehaviour projectile)
        {
            _projectile = projectile;
        }

        public void DamageTarget(Transform target)
        {
            if (_projectile.TryDamageTarget(target, out var damageable))
                damageable.TakeDamage(_projectile.Attributes.GetAttribute(ProjectileAttributes.Damage));
        }

        public IProjectileDamageStrategy Clone() => new SimpleProjectileDamage();
    }
}
