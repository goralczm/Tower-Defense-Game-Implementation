using Attributes;
using Core;
using Inventory;
using System.Collections.Generic;
using Towers.Projectiles;
using UnityEngine;

namespace Towers
{
    public class ShootStrategy : IAttackStrategy
    {
        public ProjectileBehaviour ProjectilePrefab;
        public ProjectileData DefaultProjectile;
        public List<Alignment> TargetAlignments = new();

        private TowerBehaviour _tower;
        private float _timer;
        private ProjectileData _projectile;
        private int _index;

        public void Validate()
        {
            if (DefaultProjectile.MoveStrategy.GetType().Equals(typeof(PermanentContactProjectile)))
            {
                Debug.LogError($"{typeof(PermanentContactProjectile).Name} is not compatible with {typeof(ShootStrategy).Name}");
                DefaultProjectile = null;
            }
        }

        public void Setup(TowerBehaviour tower, int index)
        {
            _tower = tower;
            _index = index % _tower.Inventory.Capacity;
            _tower.Inventory.OnSlotChanged += UpdateProjectile;

            UpdateProjectile(_tower.Inventory.Get(_index), _index);
        }

        private void UpdateProjectile(IItem projectile, int index)
        {
            if (index != _index) return;

            projectile ??= DefaultProjectile;
            _projectile = (ProjectileData)projectile;
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

            projectile.Setup(target, baseAttributes, TargetAlignments, _projectile, _projectile.MoveStrategy);
        }

        public void Dispose()
        {
            _tower.Inventory.OnSlotChanged -= UpdateProjectile;
        }

        public IAttackStrategy Clone()
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
