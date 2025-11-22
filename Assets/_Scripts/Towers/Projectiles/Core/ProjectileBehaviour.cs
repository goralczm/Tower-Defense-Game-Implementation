using Attributes;
using Core;
using Core.Cache;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilities;

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
        private IProjectileMovement _moveStrategy;
        private List<IProjectileEffect> _effects;

        public Attributes<ProjectileAttributes> Attributes => _attributes;
        public Transform Target => _target;
        public List<Alignment> CanDamageAlignments => _canDamageAlignments;
        public DamageType[] DamageTypes => _projectileData.DamageTypes;
        public ProjectileData ProjectileData => _projectileData;
        public IProjectileMovement MoveStrategy => _moveStrategy;
        public List<IProjectileEffect> Effects => _effects;
        public string Name => _projectileData.Name;
        public float ContactRadius => transform.localScale.x * .8f / 2f;
        public SpriteRenderer SpriteRenderer => _rend;

        public void Setup(Transform target, BaseAttributes<ProjectileAttributes> baseAttributes, List<Alignment> canDamageAlignments, ProjectileData projectileData, IProjectileMovement moveStrategy, List<IProjectileEffect> projectileEffects)
        {
            _target = target;
            Setup(_target.position, baseAttributes, canDamageAlignments, projectileData, moveStrategy, projectileEffects);
        }

        public void Setup(Vector2 targetPosition, BaseAttributes<ProjectileAttributes> baseAttributes, List<Alignment> canDamageAlignments, ProjectileData projectileData, IProjectileMovement moveStrategy, List<IProjectileEffect> projectileEffects)
        {
            _projectileData = projectileData;
            _targetPosition = targetPosition;
            _canDamageAlignments = canDamageAlignments;
            _moveStrategy = moveStrategy.Clone();
            _moveStrategy.Init(this);
            _rend.sprite = _projectileData.Sprite;
            _rend.color = _projectileData.Color;
            SetAttributes(new(new(), baseAttributes.Clone()));

            _effects = projectileEffects.Select(d => d.Clone()).ToList();
            _effects
                .Sort((a, b) => a.Priority.CompareTo(b.Priority));
            foreach (var effect in _effects)
            {
                effect.Init(this);
                _moveStrategy.OnTransformCollision += effect.Execute;
            }
        }

        public void SetAttributes(Attributes<ProjectileAttributes> attributes)
        {
            if (_attributes != null)
                _attributes.OnAttributesChanged -= OnAttributesChanged;

            _attributes = attributes;
            _attributes.OnAttributesChanged += OnAttributesChanged;
            OnAttributesChanged();
        }

        public void SetBaseAttributes(BaseAttributes<ProjectileAttributes> baseAttributes)
        {
            _attributes.SetBaseAttributes(baseAttributes);
        }

        public void SetTargetPosition(Vector2 target)
        {
            _targetPosition = target;
        }

        public void SetTarget(Transform target)
        {
            _target = target;
        }

        private void OnAttributesChanged()
        {

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

                transform.rotation = Helpers.RotateTowards(transform.position, _target.position, -90f);

                _moveStrategy.Move(_target.position);
            }
            else
            {
                if (_moveStrategy.DestroyIfInvalidTarget)
                {
                    DestroyProjectile();
                    return;
                }

                transform.rotation = Helpers.RotateTowards(transform.position, _targetPosition, -90f);

                _moveStrategy.Move(_targetPosition);
            }

            transform.localScale = Vector3.one * _attributes.GetAttribute(ProjectileAttributes.Size, 1f);
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
