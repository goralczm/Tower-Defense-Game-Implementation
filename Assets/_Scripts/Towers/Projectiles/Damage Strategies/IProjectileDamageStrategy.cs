using UnityEngine;

namespace Towers.Projectiles
{
    public enum DamageStrategyPriority
    {
        PreDamage,
        Damage,
        PostDamage,
    }

    public interface IProjectileDamageStrategy
    {
        public DamageStrategyPriority Priority { get; }
        public void Init(ProjectileBehaviour projectile);
        public void DamageTarget(Transform target);
        public IProjectileDamageStrategy Clone();
    }
}
