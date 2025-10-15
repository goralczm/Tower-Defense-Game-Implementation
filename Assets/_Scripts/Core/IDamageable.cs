namespace Core
{
    public enum Alignment
    {
        Hostile,
        Friendly,
        Neutral,
    }

    public interface IDamageable
    {
        public Alignment Alignment { get; }
        public void TakeDamage(float damage);
        public void Die();
    }
}
