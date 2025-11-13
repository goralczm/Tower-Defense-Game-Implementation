using ArtificeToolkit.Attributes;
using Attributes;
using Core;
using Inventory;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Towers.Projectiles;
using UnityEngine;

namespace Towers
{
    public class SpinnerStrategy : ProjectileBasedAttack
    {
        [SerializeReference, ForceArtifice] public List<IProjectileEffect> ProjectileEffects;

        private Spinner _spinner = new();
        private List<ProjectileBehaviour> _projectiles = new();

        public override string Name => "Spinjutsu";
        public override string Description => "Go cirice";

        public override void Setup(TowerBehaviour tower, int index)
        {
            base.Setup(tower, index);

            _tower.Attributes.OnAttributesChanged += OnAttributesChanged;
            OnAttributesChanged();
        }

        protected override void OnProjectileUpdated()
        {
            for (int i = _projectiles.Count - 1; i >= 0; i--)
                DestroyProjectileAt(i);

            OnAttributesChanged();
        }

        private void OnAttributesChanged()
        {
            int pointsCount = (int)_tower.Attributes.GetAttribute(TowerAttributes.ProjectilesCount);

            _spinner.SetPointsCount(pointsCount);
            _spinner.SetSpeed(_tower.Attributes.GetAttribute(TowerAttributes.RateOfFire));
            _spinner.SetRadius(_tower.Attributes.GetAttribute(TowerAttributes.Range));

            if (_projectiles.Count > pointsCount)
            {
                for (int i = _projectiles.Count - 1; i >= pointsCount; i--)
                    DestroyProjectileAt(i);
            }
            else if (_projectiles.Count < pointsCount)
            {
                for (int i = _projectiles.Count; i < pointsCount; i++)
                    CreateProjectile(_tower.transform.position);
            }
        }

        private void DestroyProjectileAt(int index)
        {
            Object.Destroy(_projectiles[index].gameObject);
            _projectiles.RemoveAt(index);
        }

        public override void Tick(float deltaTime)
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
                .Add(ProjectileAttributes.Damage, _tower.Attributes.GetAttribute(TowerAttributes.Damage) * Time.deltaTime)
                .Add(ProjectileAttributes.Range, _tower.Attributes.GetAttribute(TowerAttributes.Range))
                .Build();

            projectile.Setup(_tower.transform.position, _projectile.BaseAttributes + baseAttributes, TargetAlignments, _projectile, new PermanentContactProjectile(), _projectile.DamageStrategies.Concat(ProjectileEffects).ToList());
            _projectiles.Add(projectile);
        }

        public override void Dispose()
        {
            base.Dispose();

            _tower.Attributes.OnAttributesChanged -= OnAttributesChanged;

            for (int i = _projectiles.Count - 1; i >= 0; i--)
                Object.Destroy(_projectiles[i].gameObject);
        }

        public override IAttackStrategy Clone()
        {
            return new SpinnerStrategy()
            {
                ProjectilePrefab = ProjectilePrefab,
                DefaultProjectile = DefaultProjectile,
                TargetAlignments = TargetAlignments,
                ProjectileEffects = ProjectileEffects,
            };
        }
    }
}
