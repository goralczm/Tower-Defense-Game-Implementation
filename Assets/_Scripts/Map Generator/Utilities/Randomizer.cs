namespace MapGenerator.Utilities
{
    public static class Randomizer
    {
        public static int GetRandomSeed()
        {
            return UnityEngine.Random.Range(-100000, 100000);
        }
    }
}
