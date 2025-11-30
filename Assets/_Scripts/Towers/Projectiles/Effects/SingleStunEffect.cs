using Attributes;
using Enemies;
using Towers.Projectiles;
using UnityEngine;

namespace Towers
{
    public class SingleStunEffect : IProjectileEffect
    {
        private ProjectileBehaviour _projectile;

        public EffectPriority Priority => EffectPriority.PreDamage;

        public void Init(ProjectileBehaviour projectile)
        {
            _projectile = projectile;
        }

        public void Execute(Transform target)
        {
            if (EnemiesCache.TryGetEnemy(target, out EnemyBehaviour enemy))
                enemy.Attributes.Mediator.AddModifier(new MathAttributeModifier<EnemyAttributes>(EnemyAttributes.Speed, _projectile.Attributes.GetAttribute(ProjectileAttributes.Damage), MathOperation.Override, 0));
        }

        public IProjectileEffect Clone() => new SingleStunEffect();
    }
}
