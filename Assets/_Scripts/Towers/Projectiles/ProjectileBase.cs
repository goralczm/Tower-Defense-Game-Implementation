using ArtificeToolkit.Attributes;
using Attributes;
using Core;
using System.Collections.Generic;
using UnityEngine;

namespace Towers.Projectiles
{
    public abstract class ProjectileBase : MonoBehaviour
    {
        [SerializeField] protected BaseAttributes<ProjectileAttributes> _baseAttributes;

        protected Transform _target;
        protected Vector2 _targetPosition;
        protected Attributes<ProjectileAttributes> _attributes;
        protected List<Alignment> _canDamageAlignments = new();

        private const float STOPPING_DISTANCE = 0.05f;

        public void Setup(Transform target, BaseAttributes<ProjectileAttributes> baseAttributes, List<Alignment> canDamageAlignments)
        {
            _target = target;
            Setup(_target.position, baseAttributes, canDamageAlignments);
        }

        public void Setup(Vector2 targetPosition, BaseAttributes<ProjectileAttributes> baseAttributes, List<Alignment> canDamageAlignments)
        {
            _attributes = new(new(), _baseAttributes + baseAttributes);
            _targetPosition = targetPosition;
            _canDamageAlignments = canDamageAlignments;
            Init();
        }

        public virtual void Init() { }

        public virtual void Tick() { }

        private void Update()
        {
            _attributes.Mediator.Update(Time.deltaTime);
            Tick();
        }

        public bool IsNearTarget(Vector2 target)
        {
            return ((Vector2)transform.position - target).sqrMagnitude < STOPPING_DISTANCE;
        }

        public bool TryDamageTarget(Transform target, List<Alignment> canDamageAlignments)
        {
            if (target.TryGetComponent(out IDamageable damageable))
            {
                if (canDamageAlignments.Contains(damageable.Alignment))
                {
                    damageable.TakeDamage(_attributes.GetAttribute(ProjectileAttributes.Damage));
                    return true;
                }
            }

            return false;
        }

        public void DestroyProjectile()
        {
            Destroy(gameObject);
        }
    }
}
