using Core;
using Inventory;
using System.Collections.Generic;
using Towers.Projectiles;

namespace Towers
{
    public abstract class ProjectileBasedAttack : AttackStrategy
    {
        public ProjectileBehaviour ProjectilePrefab;
        public ProjectileData DefaultProjectile;
        public List<Alignment> TargetAlignments = new();

        protected TowerBehaviour _tower;
        protected ProjectileData _projectile;
        protected int _index;

        public override void Validate() { }

        public override void Setup(TowerBehaviour tower, int index)
        {
            _index = index;
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

            OnProjectileUpdated();
        }

        protected virtual void OnProjectileUpdated() { }


        public override void Dispose()
        {
            _tower.Inventory.OnSlotChanged -= UpdateProjectile;
        }
    }
}
