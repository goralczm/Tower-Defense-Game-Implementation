using System;
using UnityEngine;

namespace Core
{
    public enum Alignment
    {
        Hostile,
        Friendly,
        Neutral,
    }

    public enum DeathReason
    {
        Self,
        External,
    }

    public enum DamageType
    {
        Normal,
        Fire,
        Lighting,
    }

    [Serializable]
    public struct DamageData
    {
        public float Amount;
        public string Source;
        public string Target;
        public DamageType[] Types;
        public Vector2 Position;
        public float TimeStamp;

        public DamageData(float amount, string source, string target, DamageType[] types, Vector2 position)
        {
            Amount = amount;
            Source = source;
            Target = target;
            Types = types;
            Position = position;
            TimeStamp = Time.time;
        }
    }

    public interface IDamageable
    {
        public Alignment Alignment { get; }
        public void TakeDamage(float damage, DamageType[] types, string source);
        public void Die(DeathReason reason);

        public event Action<DamageData> OnDamaged;

        public static Action<IDamageable> RecordDamageRequest;
    }
}
