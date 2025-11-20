using ArtificeToolkit.Attributes;
using Attributes;
using System.Collections.Generic;
using System.Linq;
using Towers.Projectiles;
using UnityEngine;

namespace Towers
{
    [CreateAssetMenu]
    public class SpinnerStrategy : ProjectileBasedAttack
    {
        [SerializeReference, ForceArtifice] public List<IProjectileEffect> ProjectileEffects;

        private Spinner _spinner = new();
        private List<ProjectileBehaviour> _projectiles = new();

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
                var baseAttributes = TowerToProjectileAttributes.GetProjectileBaseAttributes(_tower);

                for (int i = 0; i < pointsCount; i++)
                {
                    if (i >= _projectiles.Count)
                        CreateProjectile(_tower.transform.position);
                    else
                        _projectiles[i].SetBaseAttributes(baseAttributes);
                }
            }
            else
            {
                var baseAttributes = TowerToProjectileAttributes.GetProjectileBaseAttributes(_tower);

                for (int i = 0; i < _projectiles.Count; i++)
                    _projectiles[i].SetBaseAttributes(baseAttributes);
            }
        }

        private void DestroyProjectileAt(int index)
        {
            Destroy(_projectiles[index].gameObject);
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
            ProjectileBehaviour projectile = Instantiate(ProjectilePrefab, position, Quaternion.identity);

            var baseAttributes = TowerToProjectileAttributes.GetProjectileBaseAttributes(_tower);

            projectile.Setup(_tower.transform.position, _projectile.BaseAttributes + baseAttributes, TargetAlignments, _projectile, new PermanentContactProjectile(), _projectile.DamageStrategies.Concat(ProjectileEffects).ToList());
            _projectiles.Add(projectile);
        }

        public override void Dispose()
        {
            base.Dispose();

            _tower.Attributes.OnAttributesChanged -= OnAttributesChanged;

            for (int i = _projectiles.Count - 1; i >= 0; i--)
                Destroy(_projectiles[i].gameObject);
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
