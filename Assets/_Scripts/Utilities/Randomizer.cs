namespace Utilities
{
    public class Randomizer
    {
        public static bool GetRandomBool(float probability = 0.5f)
        {
            return UnityEngine.Random.value <= probability;
        }
    }
}
