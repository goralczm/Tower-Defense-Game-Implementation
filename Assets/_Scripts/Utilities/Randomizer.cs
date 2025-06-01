using System;
using System.Collections.Generic;

public static class Randomizer
{
    public static bool GetRandomBool(float probability = 0.5f)
    {
        return UnityEngine.Random.value >= probability;
    }

    public static List<T> GetRandom<T>(this IList<T> list, int count)
    {
        if (count > list.Count) throw new ArgumentException("Count must be less/equal to list length.");

        if (count == list.Count) return (List<T>)list;
        
        List<T> outputs = new();
        
        list.Shuffle();

        for (int i = 0; i < count; i++)
        {
            outputs.Add(list[i]);
        }

        return outputs;
    }
}
