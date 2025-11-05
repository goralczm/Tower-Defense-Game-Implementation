using Attributes;
using UnityEngine;

namespace Towers.Projectiles
{
    public class AoeProjectileDamage : IProjectileDamageStrategy
    {
        private ProjectileBehaviour _projectile;

        public DamageStrategyPriority Priority => DamageStrategyPriority.Damage;

        public void Init(ProjectileBehaviour projectile)
        {
            _projectile = projectile;
        }

        public void DamageTarget(Transform target)
        {
            var hits = Physics2D.OverlapCircleAll(target.position, _projectile.Attributes.GetAttribute(ProjectileAttributes.AreaOfEffectRange));
            foreach (var hit in hits)
            {
                if (_projectile.TryDamageTarget(hit.transform, out var damageable))
                    damageable.TakeDamage(_projectile.Attributes.GetAttribute(ProjectileAttributes.Damage));
            }
        }

        public IProjectileDamageStrategy Clone() => new AoeProjectileDamage();
    }
}
