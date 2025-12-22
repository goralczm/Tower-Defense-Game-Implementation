using Attributes;
using System;
using UnityEngine;

namespace Core.Player
{
    public class Player : MonoBehaviour, IDamageable
    {
        [Header("Settings")]
        [SerializeField] private BaseAttributes<PlayerAttributes> _baseAttributes;
        [SerializeField] private CameraShake _cameraShake;

        private Attributes<PlayerAttributes> _attributes;

        public BaseAttributes<PlayerAttributes> BaseAttributes => _baseAttributes;
        public Attributes<PlayerAttributes> Attributes => _attributes;
        public Alignment Alignment => Alignment.Friendly;

        public event Action<DamageData> OnDamaged;

        public void SetAttributes(Attributes<PlayerAttributes> attributes)
        {
            _attributes = attributes;
        }

        private void Update()
        {
            if (_attributes != null)
                _attributes.Mediator.Update(Time.deltaTime);
        }

        public void TakeDamage(float damage, DamageType[] types, string source)
        {
            if (_attributes.GetAttribute(PlayerAttributes.Health) <= 0)
                return;

            var modifier = new MathAttributeModifier<PlayerAttributes>(PlayerAttributes.Health, 0f, MathOperation.Subtract, damage);

            _attributes.Mediator.AddModifier(modifier);

            if (_attributes.GetAttribute(PlayerAttributes.Health) <= 0)
                Die(DeathReason.External);
            else
                _cameraShake.DefaultShake();
        }

        public void Die(DeathReason reason)
        {
            GlobalGameEvents.OnGameEnded?.Invoke(false);
        }
    }
}
