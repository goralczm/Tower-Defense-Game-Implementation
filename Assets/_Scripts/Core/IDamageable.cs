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

    public interface IDamageable
    {
        public Alignment Alignment { get; }
        public void TakeDamage(float damage);
        public void Die(DeathReason reason);
    }
}
