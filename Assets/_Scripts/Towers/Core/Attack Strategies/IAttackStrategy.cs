namespace Towers
{
    public interface IAttackStrategy
    {
        public void Validate();
        public void Setup(TowerBehaviour tower, int index);
        public void Tick(float deltaTime);
        public void Dispose();
        public IAttackStrategy Clone();
    }
}
