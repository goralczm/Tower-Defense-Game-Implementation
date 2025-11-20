using ArtificeToolkit.Attributes;
using Attributes;
using System.Collections.Generic;
using System.Linq;
using Towers.Projectiles;
using UnityEngine;

namespace Towers
{
    [CreateAssetMenu]
    public class ShootStrategy : ProjectileBasedAttack
    {
        [SerializeReference, ForceArtifice] public List<IProjectileEffect> ProjectileEffects;

        private float _shootTimer;

        public override void Validate()
        {
            if (DefaultProjectile.MoveStrategy.GetType().Equals(typeof(PermanentContactProjectile)))
            {
                Debug.LogError($"{typeof(PermanentContactProjectile).Name} is not compatible with {typeof(ShootStrategy).Name}");
                DefaultProjectile = null;
            }
        }

        public override void Setup(TowerBehaviour tower, int index)
        {
            base.Setup(tower, index);
        }

        public override void Tick(float deltaTime)
        {
            if (_shootTimer > 0f)
            {
                _shootTimer -= deltaTime;
                return;
            }

            var targets = Targeting.Targeting.GetNTargetsInRangeByConditions(
                _tower.transform.position,
                _tower.Attributes.GetAttribute(TowerAttributes.Range),
                (int)_tower.Attributes.GetAttribute(TowerAttributes.ProjectilesCount),
                _tower.TargetingOption,
                TargetAlignments,
                true,
                _tower.transform);

            if (targets.Count > 0)
            {
                foreach (var target in targets)
                    CreateProjectile(target.Transform);

                _shootTimer = _tower.Attributes.GetAttribute(TowerAttributes.RateOfFire);
            }
        }

        private void CreateProjectile(Transform target)
        {
            ProjectileBehaviour projectile = Object.Instantiate(ProjectilePrefab, _tower.transform.position, Quaternion.identity);

            var baseAttributes = new BaseAttributesBuilder<ProjectileAttributes>()
                .Add(ProjectileAttributes.Damage, _tower.Attributes.GetAttribute(TowerAttributes.Damage))
                .Add(ProjectileAttributes.Range, _tower.Attributes.GetAttribute(TowerAttributes.Range))
                .Build();

            projectile.Setup(target, _projectile.BaseAttributes + baseAttributes, TargetAlignments, _projectile, _projectile.MoveStrategy, _projectile.DamageStrategies.Concat(ProjectileEffects).ToList());
        }

        public override IAttackStrategy Clone()
        {
            return new ShootStrategy()
            {
                ProjectilePrefab = ProjectilePrefab,
                DefaultProjectile = DefaultProjectile,
                TargetAlignments = TargetAlignments,
                ProjectileEffects = ProjectileEffects,
            };
        }
    }
}
