using Attributes;
using UnityEngine;

namespace Towers.Projectiles
{
    public class SizeBasedDamageEffect : IProjectileEffect
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
            {
                var damage = _projectile.Attributes.GetAttribute(ProjectileAttributes.Damage);
                var size = _projectile.Attributes.GetAttribute(ProjectileAttributes.Size, 1);
                damageable.TakeDamage(damage * size, _projectile.DamageTypes, _projectile.Name);
            }
        }

        public IProjectileEffect Clone() => new SizeBasedDamageEffect();
    }
}