using ArtificeToolkit.Attributes;
using Attributes;
using Core;
using System.Collections.Generic;
using Towers.Projectiles;
using UnityEngine;

namespace Towers
{
    public class ShootStrategy : IAttackStrategy
    {
        public ProjectileBehaviour ProjectilePrefab;
        public ProjectileData ProjectileData;
        [SerializeReference, ForceArtifice] public IProjectileMoveStrategy MoveStrategy;
        public List<Alignment> TargetAlignments = new();

        private TowerBehaviour _tower;
        private float _timer;

        public void Setup(TowerBehaviour tower)
        {
            _tower = tower;
        }

        public void Tick(float deltaTime)
        {
            if (_timer > 0f)
            {
                _timer -= deltaTime;
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

                _timer = _tower.Attributes.GetAttribute(TowerAttributes.RateOfFire);
            }
        }

        private void Shoot(Transform target)
        {
            ProjectileBehaviour projectile = Object.Instantiate(ProjectilePrefab, _tower.transform.position, Quaternion.identity);

            var baseAttributes = new BaseAttributesBuilder<ProjectileAttributes>()
                .Add(ProjectileAttributes.Damage, _tower.Attributes.GetAttribute(TowerAttributes.Damage))
                .Add(ProjectileAttributes.Range, _tower.Attributes.GetAttribute(TowerAttributes.Range))
                .Build();

            projectile.Setup(target, baseAttributes, TargetAlignments, ProjectileData, MoveStrategy);
        }

        public IAttackStrategy Clone()
        {
            return new ShootStrategy()
            {
                ProjectilePrefab = ProjectilePrefab,
                ProjectileData = ProjectileData,
                MoveStrategy = MoveStrategy,
                TargetAlignments = TargetAlignments,
            };
        }
    }
}
