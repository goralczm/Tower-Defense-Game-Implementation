using Attributes;
using System;
using Towers.Projectiles;
using UnityEngine;

namespace Towers
{
    public class FluctuatingSizeEffect : IProjectileEffect
    {
        public float FluctuationSpeed = 10f;

        private float _runningSum;

        public EffectPriority Priority => EffectPriority.PreDamage;

        public void Init(ProjectileBehaviour projectile)
        {
            Func<float> deltaProvider = () => _runningSum += Time.deltaTime * FluctuationSpeed;

            projectile.Attributes.Mediator.AddModifier(new FunctionAttributeModifier<ProjectileAttributes>(ProjectileAttributes.Size, 0f, v => v += Mathf.Abs(Mathf.Sin(deltaProvider()))));
        }

        public void Execute(Transform target)
        {
            //noop
        }

        public IProjectileEffect Clone() => new FluctuatingSizeEffect();
    }
}
