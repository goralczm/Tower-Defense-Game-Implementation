using System;
using System.Collections.Generic;

public static class Extensions
{
    /// <summary>
    /// Shuffles the elements of the list in place using the Fisher-Yates shuffle algorithm.
    /// Acquired from: https://stackoverflow.com/questions/273313/randomize-a-listt 25.05.2025 19:21
    /// </summary>
    public static void Shuffle<T>(this IList<T> list, int? seed = null)
    {
        Random rng = seed.HasValue ? new Random(seed.Value) : new Random();

        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
