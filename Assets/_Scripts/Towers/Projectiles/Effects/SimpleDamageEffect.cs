using Attributes;
using UnityEngine;

namespace Towers.Projectiles
{
    public class SimpleDamageEffect : IProjectileEffect
    {
        private ProjectileBehaviour _projectile;

        public EffectPriority Priority => EffectPriority.Damage;

        public void Init(ProjectileBehaviour projectile)
        {
            _projectile = projectile;
        }

        public void Execute(Transform target)
        {
            if (_projectile.TryDamageTarget(target, out var damageable))
                damageable.TakeDamage(
                    _projectile.Attributes.GetAttribute(ProjectileAttributes.Damage),
                    _projectile.DamageTypes,
                    _projectile.Name);
        }

        public IProjectileEffect Clone() => new SimpleDamageEffect();
    }
}
