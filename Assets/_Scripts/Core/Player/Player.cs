using Attributes;
using System;
using UnityEngine;

namespace Core.Player
{
    public class Player : MonoBehaviour, IDamageable
    {
        [Header("Settings")]
        [SerializeField] private BaseAttributes<PlayerAttributes> _baseAttributes;

        private Attributes<PlayerAttributes> _attributes;

        public Attributes<PlayerAttributes> Attributes => _attributes;
        public Alignment Alignment => Alignment.Friendly;

        public event Action<DamageData> OnDamaged;

        private void Awake()
        {
            _attributes = new(new(), _baseAttributes);
        }

        public void TakeDamage(float damage, DamageType[] types, string source)
        {
            if (_attributes.GetAttribute(PlayerAttributes.Health) <= 0)
                return;

            var modifier = new BasicAttributeModifier<PlayerAttributes>(PlayerAttributes.Health, 0f, v => v -= damage);

            _attributes.Mediator.AddModifier(modifier);

            if (_attributes.GetAttribute(PlayerAttributes.Health) <= 0)
                Die(DeathReason.External);
        }

        public void Die(DeathReason reason)
        {
            GlobalGameEvents.OnGameEnded?.Invoke(false);
        }
    }
}
