using Attributes;
using UnityEngine;

namespace Towers.Projectiles
{
    public class SizeSpeedScalingEffect : IProjectileEffect
    {
        private ProjectileBehaviour _projectile;
        private float _cooldownTimer;

        private const float EFFECT_COOLDOWN = .25f;

        public EffectPriority Priority => EffectPriority.PreDamage;

        public void Init(ProjectileBehaviour projectile)
        {
            _projectile = projectile;
        }

        public void Execute(Transform target)
        {
            if (_projectile.TryDamageTarget(target, out var damageable) && Time.time >= _cooldownTimer)
            {
                _projectile.Attributes.Mediator.AddModifier(new BasicAttributeModifier<ProjectileAttributes>(ProjectileAttributes.Size, 1f, v => v += .1f));
                _projectile.Attributes.Mediator.AddModifier(new BasicAttributeModifier<ProjectileAttributes>(ProjectileAttributes.Speed, 1f, v => v += .1f));
                _cooldownTimer = Time.time + EFFECT_COOLDOWN;
            }
        }

        public IProjectileEffect Clone() => new SizeSpeedScalingEffect();
    }
}
