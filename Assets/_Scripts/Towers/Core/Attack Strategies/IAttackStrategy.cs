namespace Towers
{
    public interface IAttackStrategy
    {
        public string Name { get; }
        public string Description { get; }
        public void Validate();
        public void Setup(TowerBehaviour tower, int index);
        public void Tick(float deltaTime);
        public void Dispose();
        public IAttackStrategy Clone();
    }
}
