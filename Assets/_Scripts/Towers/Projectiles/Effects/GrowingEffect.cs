using Attributes;
using System;
using Towers.Projectiles;
using UnityEngine;

namespace Towers
{
    public class GrowingEffect : IProjectileEffect
    {
        private float _runningSum;

        public EffectPriority Priority => EffectPriority.PreDamage;

        public void Init(ProjectileBehaviour projectile)
        {
            Func<float> deltaProvider = () => Mathf.Min(1.5f, _runningSum += Time.deltaTime);

            projectile.Attributes.Mediator.AddModifier(new BasicAttributeModifier<ProjectileAttributes>(ProjectileAttributes.Size, 0f, v => v += deltaProvider()));
        }

        public void Execute(Transform target)
        {
            //noop
        }

        public IProjectileEffect Clone() => new GrowingEffect();
    }
}
