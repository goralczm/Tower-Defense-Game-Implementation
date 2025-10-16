using UnityEngine;

namespace Core
{
    public class BasicDamageableEntity : MonoBehaviour, IDamageable
    {
        [SerializeField] private float _health;
        [SerializeField] private Alignment _alignment;

        public Alignment Alignment => _alignment;

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
