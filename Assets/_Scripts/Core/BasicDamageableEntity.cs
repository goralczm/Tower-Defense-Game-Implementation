using UnityEngine;

namespace Core
{
    public class BasicDamageableEntity : MonoBehaviour, ITargetable, IDamageable
    {
        [SerializeField] private float _health;
        [SerializeField] private Alignment _alignment;

        public Alignment Alignment => _alignment;
        public Transform Transform => transform;
        public int Strength => 1;
        public int Priority => -1;

        public float GetDistance(Vector2 position) => Vector2.Distance(position, transform.position);

        public void TakeDamage(float damage)
        {
            _health -= damage;

            if (_health <= 0f)
                Die();
        }

        public void Die()
        {
            Destroy(gameObject);
        }
    }
}
