namespace Towers
{
    public interface IAttackStrategy
    {
        public void Validate();
        public void Setup(TowerBehaviour tower);
        public void Tick(float deltaTime);
        public IAttackStrategy Clone();
    }
}
