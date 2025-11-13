using ArtificeToolkit.Attributes;
using Attributes;
using System.Collections.Generic;
using System.Linq;
using Towers.Projectiles;
using UnityEngine;

namespace Towers
{
    public class LaserStrategy : ProjectileBasedAttack
    {
        [SerializeReference, ForceArtifice] public List<IProjectileEffect> ProjectileEffects;

        private List<ProjectileBehaviour> _projectiles = new();

        public override string Name => "Laseru dase";
        public override string Description => "Bzzzz";

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
            int projectilesCount = (int)_tower.Attributes.GetAttribute(TowerAttributes.ProjectilesCount);

            if (_projectiles.Count > projectilesCount)
            {
                for (int i = _projectiles.Count - 1; i >= projectilesCount; i--)
                    DestroyProjectileAt(i);
            }
            else if (_projectiles.Count < projectilesCount)
            {
                for (int i = _projectiles.Count; i < projectilesCount; i++)
                    CreateProjectile();
            }
        }

        private void DestroyProjectileAt(int index)
        {
            Object.Destroy(_projectiles[index].gameObject);
            _projectiles.RemoveAt(index);
        }

        public override void Tick(float deltaTime)
        {
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
                for (int i = 0; i < _projectiles.Count; i++)
                {
                    if (i <= targets.Count - 1)
                    {
                        _projectiles[i].SetTarget(targets[i].Transform);
                        _projectiles[i].gameObject.SetActive(true);
                    }
                    else
                        _projectiles[i].gameObject.SetActive(false);
                }
            }
            else
            {
                for (int i = 0; i < _projectiles.Count; i++)
                {
                    _projectiles[i].gameObject.SetActive(false);
                }
            }
        }

        private void CreateProjectile()
        {
            ProjectileBehaviour projectile = Object.Instantiate(ProjectilePrefab, _tower.transform.position, Quaternion.identity);

            var baseAttributes = new BaseAttributesBuilder<ProjectileAttributes>()
                .Add(ProjectileAttributes.Damage, _tower.Attributes.GetAttribute(TowerAttributes.Damage) * Time.deltaTime)
                .Add(ProjectileAttributes.Range, _tower.Attributes.GetAttribute(TowerAttributes.Range))
                .Build();

            projectile.Setup(_tower.transform.position, _projectile.BaseAttributes + baseAttributes, TargetAlignments, _projectile, new LaserProjectile(), _projectile.DamageStrategies.Concat(ProjectileEffects).ToList());
            _projectiles.Add(projectile);
        }

        public override IAttackStrategy Clone()
        {
            return new LaserStrategy()
            {
                ProjectilePrefab = ProjectilePrefab,
                DefaultProjectile = DefaultProjectile,
                TargetAlignments = TargetAlignments,
                ProjectileEffects = ProjectileEffects,
            };
        }
    }
}
