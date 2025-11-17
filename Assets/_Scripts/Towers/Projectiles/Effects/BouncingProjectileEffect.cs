using Attributes;
using UnityEngine;

namespace Towers.Projectiles
{
    public class BouncingProjectileEffect : IProjectileEffect
    {
        private ProjectileBehaviour _projectile;

        public EffectPriority Priority => EffectPriority.PostDamage;

        public void Init(ProjectileBehaviour projectile)
        {
            _projectile = projectile;
        }

        public void Execute(Transform target)
        {
            if (_projectile.TryDamageTarget(target, out var damageable))
            {
                if (_projectile.Attributes.GetAttribute(ProjectileAttributes.Bounces, 0) > 0)
                {
                    var targets = Targeting.Targeting.GetTargetsInRange(_projectile.transform.position, 1f, _projectile.CanDamageAlignments, target);

                    if (targets.Count > 0)
                    {
                        var projectile = Object.Instantiate(
                            _projectile,
                            target.transform.position,
                            _projectile.transform.rotation);

                        projectile.Setup(
                            targets[0].Transform,
                            _projectile.Attributes.BaseAttributes,
                            _projectile.CanDamageAlignments,
                            _projectile.ProjectileData,
                            new HomingProjectile(),
                            _projectile.Effects);

                        projectile.SetAttributes(_projectile.Attributes.Clone());
                        projectile.Attributes.Mediator.AddModifier(new BasicAttributeModifier<ProjectileAttributes>(ProjectileAttributes.Bounces, 0f, v => v = v - 1));
                    }
                }
            }
        }

        public IProjectileEffect Clone() => new BouncingProjectileEffect();
    }
}
