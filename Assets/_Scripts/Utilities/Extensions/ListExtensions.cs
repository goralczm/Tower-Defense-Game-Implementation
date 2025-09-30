using System;
using System.Collections.Generic;

namespace Utilities.Extensions
{
    public static class ListExtensions
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

        public static List<T> GetRandom<T>(this IList<T> list, int count)
        {
            if (count > list.Count) throw new ArgumentException("Count must be less/equal to list length.");

            if (count == list.Count) return (List<T>)list;

            List<T> outputs = new();

            list.Shuffle();

            for (int i = 0; i < count; i++)
                outputs.Add(list[i]);

            return outputs;
        }
    }
}
