using Attributes;
using System;
using Towers.Projectiles;
using UnityEngine;

namespace Towers
{
    public class SizeBasedSpeedEffect : IProjectileEffect
    {
        public EffectPriority Priority => EffectPriority.PreDamage;

        public void Init(ProjectileBehaviour projectile)
        {
            Func<float> sizeProvider = () => projectile.transform.localScale.x * 2f;

            projectile.Attributes.Mediator.AddModifier(new BasicAttributeModifier<ProjectileAttributes>(ProjectileAttributes.Speed, 0f, v => v += sizeProvider()));
        }

        public void Execute(Transform target)
        {
            //noop
        }

        public IProjectileEffect Clone() => new SizeBasedSpeedEffect();
    }
}
