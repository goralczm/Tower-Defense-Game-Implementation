using ArtificeToolkit.Attributes;
using Attributes;
using Core;
using System.Collections.Generic;
using Towers.Projectiles;
using UnityEngine;

namespace Towers
{
    public class SpinnerStrategy : IAttackStrategy
    {
        public ProjectileBehaviour ProjectilePrefab;
        public ProjectileData ProjectileData;
        public List<Alignment> TargetAlignments = new();

        private TowerBehaviour _tower;
        private Spinner _spinner;
        private List<ProjectileBehaviour> _projectiles = new();

        public void Setup(TowerBehaviour tower)
        {
            _tower = tower;
            _spinner = new Spinner();
            _tower.Attributes.OnAttributesChanged += OnAttributesChanged;
            OnAttributesChanged();

            for (int i = 0; i < _spinner.GetPointsCount(); i++)
                CreateProjectile();
        }

        private void OnAttributesChanged()
        {
            _spinner.SetPointsCount((int)_tower.Attributes.GetAttribute(TowerAttributes.ProjectilesCount));
            _spinner.SetSpeed(ProjectileData.BaseAttributes.GetBaseAttribute(ProjectileAttributes.Speed));
            _spinner.SetRadius(_tower.Attributes.GetAttribute(TowerAttributes.Range));
        }

        public void Tick(float deltaTime)
        {
            _spinner.Update(deltaTime);

            var positions = _spinner.GetAllPointsPositions(_tower.transform.position);

            for (int i = 0; i < _projectiles.Count; i++)
                _projectiles[i].SetTargetPosition(positions[i]);
        }

        private void CreateProjectile()
        {
            ProjectileBehaviour projectile = Object.Instantiate(ProjectilePrefab, _tower.transform.position, Quaternion.identity);

            var baseAttributes = new BaseAttributesBuilder<ProjectileAttributes>()
                .Add(ProjectileAttributes.Damage, _tower.Attributes.GetAttribute(TowerAttributes.Damage))
                .Add(ProjectileAttributes.Range, _tower.Attributes.GetAttribute(TowerAttributes.Range))
                .Build();

            projectile.Setup(_tower.transform.position, baseAttributes, TargetAlignments, ProjectileData, new PermanentContactProjectile());
            _projectiles.Add(projectile);
        }

        public IAttackStrategy Clone()
        {
            return new SpinnerStrategy()
            {
                ProjectilePrefab = ProjectilePrefab,
                ProjectileData = ProjectileData,
                TargetAlignments = TargetAlignments,
            };
        }
    }
}
