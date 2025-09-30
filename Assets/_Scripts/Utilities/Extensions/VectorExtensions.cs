using UnityEngine;

namespace Utilities.Extensions
{
    public static class VectorExtensions
    {
        public static Vector2 Add(this Vector2 first, Vector3 second)
        {
            return new Vector2(first.x + second.x, first.y + second.y);
        }
    }

}
