using System.Collections.Generic;
using UnityEngine;

namespace Paths.Utilities
{
    public class Helpers : MonoBehaviour
    {
        public static float CalculatePathLength(List<Vector2> waypoints)
        {
            float length = 0;

            for (int i = 0; i < waypoints.Count - 1; i++)
                length += Vector2.Distance(waypoints[i], waypoints[i + 1]);

            return length;
        }
    }
}
