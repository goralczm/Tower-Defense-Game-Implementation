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
            Func<float> sizeProvider = () => projectile.transform.localScale.x * 5f;

            projectile.Attributes.Mediator.AddModifier(new FunctionAttributeModifier<ProjectileAttributes>(ProjectileAttributes.Speed, 0f, v => v += sizeProvider()));
        }

        public void Execute(Transform target)
        {
            //noop
        }

        public IProjectileEffect Clone() => new SizeBasedSpeedEffect();
    }
}
