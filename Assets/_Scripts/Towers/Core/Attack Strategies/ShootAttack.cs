using ArtificeToolkit.Attributes;
using Attributes;
using System.Collections.Generic;
using System.Linq;
using Towers.Projectiles;
using UnityEngine;
using Utilities;

namespace Towers
{
    [CreateAssetMenu(menuName = "Towers/Attacks/Shoot Attack")]
    public class ShootAttack : ProjectileBasedAttack
    {
        [SerializeReference, ForceArtifice] public List<IProjectileEffect> ProjectileEffects;

        private float _shootTimer;

        public override void Validate()
        {
            if (DefaultProjectile.MoveStrategy.GetType().Equals(typeof(PermanentContactProjectile)))
            {
                Debug.LogError($"{typeof(PermanentContactProjectile).Name} is not compatible with {typeof(ShootAttack).Name}");
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
                var targetRotation = Helpers.RotateTowards(_tower.transform.position, targets[0].Transform.position, -90f);
                _tower.transform.rotation = targetRotation;

                foreach (var target in targets)
                    CreateProjectile(target.Transform);

                _shootTimer = _tower.Attributes.GetAttribute(TowerAttributes.RateOfFire);
            }
        }

        private void CreateProjectile(Transform target)
        {
            ProjectileBehaviour projectile = Object.Instantiate(ProjectilePrefab, _tower.transform.position, Quaternion.identity);

            var baseAttributes = TowerToProjectileAttributes.GetProjectileBaseAttributes(_tower);

            projectile.Setup(target, _projectile.BaseAttributes + baseAttributes, TargetAlignments, _projectile, _projectile.MoveStrategy, _projectile.DamageStrategies.Concat(ProjectileEffects).ToList());
        }

        public override IAttackStrategy Clone()
        {
            return new ShootAttack()
            {
                ProjectilePrefab = ProjectilePrefab,
                DefaultProjectile = DefaultProjectile,
                TargetAlignments = TargetAlignments,
                ProjectileEffects = ProjectileEffects,
            };
        }
    }
}
