using UnityEngine;

namespace Towers.Projectiles
{
    public enum EffectPriority
    {
        PreDamage,
        Damage,
        PostDamage,
    }

    public interface IProjectileEffect
    {
        public EffectPriority Priority { get; }
        public void Init(ProjectileBehaviour projectile);
        public void Execute(Transform target);
        public IProjectileEffect Clone();
    }
}
