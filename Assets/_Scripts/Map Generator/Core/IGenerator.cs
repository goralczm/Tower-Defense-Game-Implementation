namespace MapGenerator.Core
{
    public interface IGenerator
    {
        public MapLayout Generate(MapLayout layout);

        public void Cleanup();
    }
}
