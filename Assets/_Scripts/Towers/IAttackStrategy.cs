namespace Towers
{
    public interface IAttackStrategy
    {
        public void Setup();
        public void Tick();
        public void Cleanup();
    }
}
