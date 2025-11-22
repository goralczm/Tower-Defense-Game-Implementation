    using Attributes;
using ObjectPooling;
using UnityEngine;

namespace Towers.Projectiles
{
    public class AoeDamageEffect : IProjectileEffect
    {
        private ProjectileBehaviour _projectile;

        public EffectPriority Priority => EffectPriority.Damage;

        public void Init(ProjectileBehaviour projectile)
        {
            _projectile = projectile;
        }

        public void Execute(Transform target)
        {
            PoolManager.Instance.SpawnFromPool("Explosion", target.transform.position, Quaternion.identity);
            var hits = Physics2D.OverlapCircleAll(target.position, _projectile.Attributes.GetAttribute(ProjectileAttributes.AreaOfEffectRange));
            foreach (var hit in hits)
            {
                if (_projectile.TryDamageTarget(hit.transform, out var damageable))
                    damageable.TakeDamage(_projectile.Attributes.GetAttribute(ProjectileAttributes.Damage), _projectile.DamageTypes, _projectile.Name);
            }
        }

        public IProjectileEffect Clone() => new AoeDamageEffect();
    }
}
