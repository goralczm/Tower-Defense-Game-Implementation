using ArtificeToolkit.Attributes;
using Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Towers.Projectiles;
using UnityEngine;

namespace Towers
{
    [CreateAssetMenu(menuName = "Towers/Attacks/Landminer Attack")]
    public class LandminerAttack : ProjectileBasedAttack
    {
        [SerializeReference, ForceArtifice] public List<IProjectileEffect> ProjectileEffects;

        private List<ProjectileBehaviour> _projectiles = new();

        private float _spawnTimer;

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
            _spawnTimer = Time.time;
        }

        private void DestroyProjectileAt(int index)
        {
            Destroy(_projectiles[index].gameObject);
            _projectiles.RemoveAt(index);
        }

        public override void Tick(float deltaTime)
        {
            if (Time.time < _spawnTimer) return;

            for (int i = _projectiles.Count - 1; i >= 0; i--)
            {
                if (_projectiles[i] == null)
                    _projectiles.RemoveAt(i);
            }

            int missingProjectiles = (int)_tower.Attributes.GetAttribute(TowerAttributes.ProjectilesCount) - _projectiles.Count;

            if (missingProjectiles <= 0) return;

            var positions = Targeting.Targeting.GetPathPointsInRange(_tower.transform.position, _tower.Attributes.GetAttribute(TowerAttributes.Range), 16);
            var usedIndexes = new HashSet<int>();

            for (int i = 0; i < missingProjectiles; i++)
            {
                var randomIndex = Random.Range(0, positions.Count - 1);

                while (usedIndexes.Count < positions.Count && usedIndexes.Contains(randomIndex))
                    randomIndex = Random.Range(0, positions.Count - 1);

                CreateProjectile(positions[randomIndex]);
            }

            _spawnTimer = Time.time + _tower.Attributes.GetAttribute(TowerAttributes.RateOfFire);
        }

        private void CreateProjectile(Vector2 position)
        {
            ProjectileBehaviour projectile = Instantiate(ProjectilePrefab, _tower.transform.position, Quaternion.identity);

            var baseAttributes = TowerToProjectileAttributes.GetProjectileBaseAttributes(_tower);

            projectile.Setup(position, _projectile.BaseAttributes + baseAttributes, TargetAlignments, _projectile, new TemporaryContactProjectile(), _projectile.DamageStrategies.Concat(ProjectileEffects).ToList());
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
            return new LandminerAttack()
            {
                ProjectilePrefab = ProjectilePrefab,
                DefaultProjectile = DefaultProjectile,
                TargetAlignments = TargetAlignments,
                ProjectileEffects = ProjectileEffects,
            };
        }
    }
}
