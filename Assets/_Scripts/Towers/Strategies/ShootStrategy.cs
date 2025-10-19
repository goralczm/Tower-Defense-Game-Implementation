using Attributes;
using Towers.Projectiles;
using UnityEngine;

namespace Towers
{
    public class ShootStrategy : IAttackStrategy
    {
        public ProjectileBase Projectile;

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

            var enemies = Targeting.Targeting.GetNEnemiesInRangeByConditions(
                _tower.transform.position,
                _tower.Attributes.GetAttribute(Attributes.TowerAttributes.Range),
                (int)_tower.Attributes.GetAttribute(Attributes.TowerAttributes.ProjectilesCount),
                _tower.TargetingOption,
                true);

            if (enemies.Count > 0)
            {
                foreach (var enemy in enemies)
                    Shoot(enemy.transform);

                _timer = _tower.Attributes.GetAttribute(Attributes.TowerAttributes.RateOfFire);
            }
        }

        private void Shoot(Transform target)
        {
            ProjectileBase projectile = Object.Instantiate(Projectile, _tower.transform.position, Quaternion.identity);

            var baseAttributes = new BaseAttributesBuilder<ProjectileAttributes>()
                .Add(ProjectileAttributes.Damage, _tower.Attributes.GetAttribute(Attributes.TowerAttributes.Damage))
                .Add(ProjectileAttributes.Range, _tower.Attributes.GetAttribute(Attributes.TowerAttributes.Range))
                .Build();

            projectile.Setup(target, baseAttributes);
        }
    }
}
