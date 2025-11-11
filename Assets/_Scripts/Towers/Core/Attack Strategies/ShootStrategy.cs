using Attributes;
using Towers.Projectiles;
using UnityEngine;

namespace Towers
{
    public class ShootStrategy : ProjectileBasedAttack
    {
        private float _shootTimer;

        public override string Name => "Opps Stoppa";
        public override string Description => "Pew Pew";

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
                    Shoot(target.Transform);

                _shootTimer = _tower.Attributes.GetAttribute(TowerAttributes.RateOfFire);
            }
        }

        private void Shoot(Transform target)
        {
            ProjectileBehaviour projectile = Object.Instantiate(ProjectilePrefab, _tower.transform.position, Quaternion.identity);

            var baseAttributes = new BaseAttributesBuilder<ProjectileAttributes>()
                .Add(ProjectileAttributes.Damage, _tower.Attributes.GetAttribute(TowerAttributes.Damage))
                .Add(ProjectileAttributes.Range, _tower.Attributes.GetAttribute(TowerAttributes.Range))
                .Build();

            projectile.Setup(target, baseAttributes, TargetAlignments, _projectile, _projectile.MoveStrategy);
        }

        public override IAttackStrategy Clone()
        {
            return new ShootStrategy()
            {
                ProjectilePrefab = ProjectilePrefab,
                DefaultProjectile = DefaultProjectile,
                TargetAlignments = TargetAlignments,
            };
        }
    }
}
