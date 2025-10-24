using Attributes;
using UnityEngine;
using Utilities;

namespace Towers.Projectiles
{
    public class BiggoProjectileStrategy : IProjectileDamageStrategy
    {
        private ProjectileBehaviour _projectile;
        private float _cooldownTimer;

        private const float EFFECT_COOLDOWN = .2f;

        public DamageStrategyPriority Priority => DamageStrategyPriority.PreDamage;

        public void Init(ProjectileBehaviour projectile)
        {
            _projectile = projectile;
        }

        public void DamageTarget(Transform target)
        {
            if (_projectile.TryDamageTarget(target, out var damageable) && Time.time >= _cooldownTimer)
            {
                _projectile.Attributes.Mediator.AddModifier(new BasicAttributeModifier<ProjectileAttributes>(ProjectileAttributes.Size, 1f, v => v += .2f));
                _projectile.Attributes.Mediator.AddModifier(new BasicAttributeModifier<ProjectileAttributes>(ProjectileAttributes.Speed, 1f, v => v += .5f));
                _cooldownTimer = Time.time + EFFECT_COOLDOWN;
            }
        }

        public IProjectileDamageStrategy Clone() => new BiggoProjectileStrategy();
    }
}
