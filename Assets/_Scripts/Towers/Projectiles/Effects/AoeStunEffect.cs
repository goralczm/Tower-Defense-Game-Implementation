using Attributes;
using Enemies;
using ObjectPooling;
using Towers.Projectiles;
using UnityEngine;

namespace Towers
{
    public class AoeStunEffect : IProjectileEffect
    {
        private ProjectileBehaviour _projectile;

        public EffectPriority Priority => EffectPriority.Damage;

        public void Init(ProjectileBehaviour projectile)
        {
            _projectile = projectile;
        }

        public void Execute(Transform target)
        {
            PoolManager.Instance.SpawnFromPool("Sparks", target.transform.position, Quaternion.identity);
            var hits = Physics2D.OverlapCircleAll(target.position, _projectile.Attributes.GetAttribute(ProjectileAttributes.AreaOfEffectRange));
            foreach (var hit in hits)
            {
                if (EnemiesCache.TryGetEnemy(hit, out EnemyBehaviour enemy))
                    enemy.Attributes.Mediator.AddModifier(new BasicAttributeModifier<EnemyAttributes>(EnemyAttributes.Speed, _projectile.Attributes.GetAttribute(ProjectileAttributes.Damage), v => 0));
            }
        }

        public IProjectileEffect Clone() => new AoeStunEffect();
    }
}
