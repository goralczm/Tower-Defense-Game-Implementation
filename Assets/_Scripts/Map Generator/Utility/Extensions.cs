using System;
using System.Collections.Generic;

public static class Extensions
{
    /// <summary>
    /// Shuffles the elements of the list in place using the Fisher-Yates shuffle algorithm.
    /// Acquired from: https://stackoverflow.com/questions/273313/randomize-a-listt 25.05.2025 19:21
    /// </summary>
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }
}
