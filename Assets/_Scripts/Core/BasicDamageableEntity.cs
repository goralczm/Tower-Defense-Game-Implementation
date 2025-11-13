using System;
using UnityEngine;

namespace Core
{
    public class BasicDamageableEntity : MonoBehaviour, ITargetable, IDamageable
    {
        [SerializeField] private float _health;
        [SerializeField] private Alignment _alignment;

        public event Action<DamageData> OnDamaged;

        public Alignment Alignment => _alignment;
        public Transform Transform => transform;
        public int Strength => 1;
        public int TargetingPriority => -1;

        public float GetDistance(Vector2 position) => Vector2.Distance(position, transform.position);

        private void Start()
        {
            IDamageable.RecordDamageRequest?.Invoke(this);
        }

        public void TakeDamage(float damage, DamageType[] types, string source)
        {
            OnDamaged?.Invoke(new(
                Mathf.Min(_health, damage),
                source, transform.name,
                types,
                transform.position));

            _health -= damage;

            if (_health <= 0f)
                Die(DeathReason.External);
        }

        public void Die(DeathReason reason)
        {
            Destroy(gameObject);
        }
    }
}
