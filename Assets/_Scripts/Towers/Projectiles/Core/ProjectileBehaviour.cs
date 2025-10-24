using ArtificeToolkit.Attributes;
using Attributes;
using Core;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Towers.Projectiles
{
    public class ProjectileBehaviour : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SpriteRenderer _rend;

        private ProjectileData _projectileData;
        private Transform _target;
        private Vector2 _targetPosition;
        private Attributes<ProjectileAttributes> _attributes;
        private List<Alignment> _canDamageAlignments = new();
        private IProjectileMoveStrategy _moveStrategy;
        private List<IProjectileDamageStrategy> _damageStrategies;

        public Attributes<ProjectileAttributes> Attributes => _attributes;
        public Transform Target => _target;
        public List<Alignment> CanDamageAlignments => _canDamageAlignments;

        public void Setup(Transform target, BaseAttributes<ProjectileAttributes> baseAttributes, List<Alignment> canDamageAlignments, ProjectileData projectileData, IProjectileMoveStrategy moveStrategy)
        {
            _target = target;
            Setup(_target.position, baseAttributes, canDamageAlignments, projectileData, moveStrategy);
        }

        public void Setup(Vector2 targetPosition, BaseAttributes<ProjectileAttributes> baseAttributes, List<Alignment> canDamageAlignments, ProjectileData projectileData, IProjectileMoveStrategy moveStrategy)
        {
            _projectileData = projectileData;
            _attributes = new(new(), _projectileData.BaseAttributes + baseAttributes);
            _targetPosition = targetPosition;
            _canDamageAlignments = canDamageAlignments;
            _moveStrategy = moveStrategy.Clone();
            _damageStrategies = _projectileData.DamageStrategies.Select(d => d.Clone()).ToList();
            _moveStrategy.Init(this);
            _damageStrategies
                .Sort((a, b) => a.Priority.CompareTo(b.Priority));
            foreach (var damageStrategy in _damageStrategies)
            {
                damageStrategy.Init(this);
                _moveStrategy.OnTransformCollision += damageStrategy.DamageTarget;
            }
            _rend.sprite = _projectileData.Sprite;
            _rend.color = _projectileData.Color;
            _attributes.OnAttributesChanged += OnAttributesChanged;
            OnAttributesChanged();
        }

        public void SetTargetPosition(Vector2 target)
        {
            _targetPosition = target;
        }

        private void OnAttributesChanged()
        {
            transform.localScale = Vector3.one * _attributes.GetAttribute(ProjectileAttributes.Size, 1f);
        }

        private void Update()
        {
            _attributes.Mediator.Update(Time.deltaTime);

            if (_target)
            {
                if (_moveStrategy.DestroyIfInvalidTarget && !_target.gameObject.activeSelf)
                {
                    DestroyProjectile();
                    return;
                }

                _moveStrategy.Move(_target.position);
            }
            else
            {
                if (_moveStrategy.DestroyIfInvalidTarget)
                {
                    DestroyProjectile();
                    return;
                }

                _moveStrategy.Move(_targetPosition);
            }
        }

        public bool IsNearTarget(Vector2 target)
        {
            return ((Vector2)transform.position - target).sqrMagnitude < 0.05f;
        }

        public bool TryDamageTarget(Transform target, out IDamageable damageable, List<Alignment> canDamageAlignments = null)
        {
            canDamageAlignments ??= _canDamageAlignments;

            return DamageableCache.TryGetDamageable(target, out damageable) && canDamageAlignments.Contains(damageable.Alignment);
        }

        public void DestroyProjectile()
        {
            Destroy(gameObject);
        }
    }
}
