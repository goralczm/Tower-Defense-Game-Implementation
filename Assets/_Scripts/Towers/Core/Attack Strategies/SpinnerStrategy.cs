using ArtificeToolkit.Attributes;
using Attributes;
using Core;
using Inventory;
using System.Collections.Generic;
using Towers.Projectiles;
using UnityEngine;

namespace Towers
{
    public class SpinnerStrategy : IAttackStrategy
    {
        public ProjectileBehaviour ProjectilePrefab;
        public ProjectileData DefaultProjectile;
        public List<Alignment> TargetAlignments = new();

        private TowerBehaviour _tower;
        private Spinner _spinner;
        private List<ProjectileBehaviour> _projectiles = new();
        private ProjectileData _projectile;
        private int _index;

        public void Validate()
        {
            //noop
        }

        public void Setup(TowerBehaviour tower, int index)
        {
            _tower = tower;
            _index = index % _tower.Inventory.Capacity;
            _spinner = new Spinner();
            _tower.Attributes.OnAttributesChanged += OnAttributesChanged;
            OnAttributesChanged();

            _tower.Inventory.OnSlotChanged += UpdateProjectile;
            UpdateProjectile(_tower.Inventory.Get(_index), _index);
        }

        private void UpdateProjectile(IItem projectile, int index)
        {
            if (index != _index) return;

            for (int i = _projectiles.Count - 1; i >= 0; i--)
                Object.Destroy(_projectiles[i].gameObject);

            _projectiles.Clear();

            projectile ??= DefaultProjectile;
            _projectile = (ProjectileData)projectile;

            var points = _spinner.GetAllPointsPositions(_tower.transform.position);
            for (int i = 0; i < points.Length; i++)
                CreateProjectile(points[i]);
        }

        private void OnAttributesChanged()
        {
            _spinner.SetPointsCount((int)_tower.Attributes.GetAttribute(TowerAttributes.ProjectilesCount));
            _spinner.SetSpeed(_tower.Attributes.GetAttribute(TowerAttributes.RateOfFire));
            _spinner.SetRadius(_tower.Attributes.GetAttribute(TowerAttributes.Range));
        }

        public void Tick(float deltaTime)
        {
            _spinner.Update(deltaTime);

            var positions = _spinner.GetAllPointsPositions(_tower.transform.position);

            for (int i = 0; i < _projectiles.Count; i++)
                _projectiles[i].SetTargetPosition(positions[i]);
        }

        private void CreateProjectile(Vector2 position)
        {
            ProjectileBehaviour projectile = Object.Instantiate(ProjectilePrefab, position, Quaternion.identity);

            var baseAttributes = new BaseAttributesBuilder<ProjectileAttributes>()
                .Add(ProjectileAttributes.Damage, _tower.Attributes.GetAttribute(TowerAttributes.Damage))
                .Add(ProjectileAttributes.Range, _tower.Attributes.GetAttribute(TowerAttributes.Range))
                .Build();

            projectile.Setup(_tower.transform.position, baseAttributes, TargetAlignments, _projectile, new PermanentContactProjectile());
            _projectiles.Add(projectile);
        }

        public void Dispose()
        {
            _tower.Attributes.OnAttributesChanged -= OnAttributesChanged;
            _tower.Inventory.OnSlotChanged -= UpdateProjectile;

            for (int i = _projectiles.Count - 1; i >= 0; i--)
                Object.Destroy(_projectiles[i].gameObject);
        }

        public IAttackStrategy Clone()
        {
            return new SpinnerStrategy()
            {
                ProjectilePrefab = ProjectilePrefab,
                DefaultProjectile = DefaultProjectile,
                TargetAlignments = TargetAlignments,
            };
        }
    }
}
