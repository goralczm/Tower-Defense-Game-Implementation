namespace Towers
{
    public interface IAttackStrategy
    {
        public void Setup(TowerBehaviour tower);
        public void Tick(float deltaTime);
        public IAttackStrategy Clone();
    }
}
