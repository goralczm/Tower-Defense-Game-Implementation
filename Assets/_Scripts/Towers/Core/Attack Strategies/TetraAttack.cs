using ArtificeToolkit.Attributes;
using Attributes;
using System.Collections.Generic;
using System.Linq;
using Towers.Projectiles;
using UnityEngine;

namespace Towers
{
    [CreateAssetMenu(menuName = "Towers/Attacks/Tetra Attack")]
    public class TetraAttack : ProjectileBasedAttack
    {
        [SerializeReference, ForceArtifice] public IProjectileMovement MoveStrategy;
        [SerializeReference, ForceArtifice] public List<IProjectileEffect> ProjectileEffects = new();

        private float _shootTimer;
        private Spinner _spinner;

        public override void Setup(TowerBehaviour tower, int index)
        {
            base.Setup(tower, index);

            _spinner = new Spinner();
            _tower.Attributes.OnAttributesChanged += OnAttributesChanged;
            OnAttributesChanged();
        }

        private void OnAttributesChanged()
        {
            _spinner.SetPointsCount((int)_tower.Attributes.GetAttribute(TowerAttributes.ProjectilesCount, 0));
            _spinner.SetRadius(_tower.Attributes.GetAttribute(TowerAttributes.Range, 0));
        }

        public override void Tick(float deltaTime)
        {
            if (_shootTimer > 0f)
            {
                _shootTimer -= deltaTime;
                return;
            }

            Shoot();
            _shootTimer = _tower.Attributes.GetAttribute(TowerAttributes.RateOfFire);
        }

        private void Shoot()
        {
            var points = _spinner.GetAllPointsPositions(_tower.transform.position);

            foreach (var point in points)
                Shoot(point);
        }

        private void Shoot(Vector2 position)
        {
            ProjectileBehaviour projectile = Instantiate(ProjectilePrefab, _tower.transform.position, Quaternion.identity);

            var baseAttributes = TowerToProjectileAttributes.GetProjectileBaseAttributes(_tower);

            projectile.Setup(position, _projectile.BaseAttributes + baseAttributes, TargetAlignments, _projectile, MoveStrategy, _projectile.DamageStrategies.Concat(ProjectileEffects).ToList());
        }

        public override void Dispose()
        {
            base.Dispose();

            _tower.Attributes.OnAttributesChanged -= OnAttributesChanged;
        }

        public override IAttackStrategy Clone()
        {
            return new TetraAttack()
            {
                ProjectilePrefab = ProjectilePrefab,
                DefaultProjectile = DefaultProjectile,
                TargetAlignments = TargetAlignments,
                MoveStrategy = MoveStrategy,
                ProjectileEffects = ProjectileEffects,
            };
        }
    }
}
